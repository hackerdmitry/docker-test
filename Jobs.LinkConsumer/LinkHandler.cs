using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jobs.LinkConsumer.Configurations;
using Jobs.LinkConsumer.Events;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;

namespace Jobs.LinkConsumer
{
    public class LinkHandler
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly GlobalSettings _globalSettings;
        private readonly IDistributedCache _distributedCache;

        public LinkHandler(RabbitMqConfiguration configuration,
                           GlobalSettings globalSettings,
                           IDistributedCache distributedCache)
        {
            _configuration = configuration;
            _globalSettings = globalSettings;
            _distributedCache = distributedCache;
        }

        public async Task HandleAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.Host,
                UserName = _configuration.Username,
                Password = _configuration.Password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Console.WriteLine(" [x] Connected to RabbitMQ");

                channel.QueueDeclarePassive(_configuration.QueueName);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var linkEvent = JsonConvert.DeserializeObject<LinkEvent>(message);

                    Console.WriteLine(" [x] Received {0}", message);

                    var response = await GetResponseFromRedisAsync(linkEvent.Href);
                    if (response == null)
                    {
                        try
                        {
                            response = await HandleLinkEventAsync(linkEvent);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" [!] Error");
                            Console.WriteLine(e);
                        }
                    }

                    await LinkStepAsync(linkEvent.Id, response);
                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: _configuration.QueueName,
                    autoAck: false,
                    consumer: consumer);

                await Task.Delay(Timeout.Infinite);
            }
        }

        private async Task<LinkResponse> HandleLinkEventAsync(LinkEvent @event)
        {
            for (var i = 0; i < 5; i++)
            {
                Console.WriteLine($"Попытка #{i + 1}: {@event.Href}");
                do
                {
                    var response = await LinkStepAsync(@event.Id);
                    if (response.IsSuccessful)
                    {
                        var tact = double.Parse(response.Content);
                        if (tact == -1)
                        {
                            return await GetAndSaveResponseAsync(@event.Href);
                        }

                        await Task.Delay((int) (tact * 1000));
                    }
                    else
                    {
                        Console.WriteLine($" [!] Not successful - {response.StatusCode}");
                        break;
                    }
                }
                while (true);

                await Task.Delay(5000);
            }

            throw new Exception("Сервер не отвечает");
        }

        private async Task<IRestResponse> LinkStepAsync(int id, LinkResponse response = null)
        {
            var uri = $"{_globalSettings.PublicHost}/api/link/step?id={id}";
            var client = new RestClient(uri) { Timeout = -1 };
            var request = new RestRequest(Method.PUT);

            if (response == null)
            {
                request.AddJsonBody("{}");
            }
            else
            {
                request.AddJsonBody(response);
            }

            Console.WriteLine($"Отправил request=[id={id} response={response}]");
            return await client.ExecuteAsync(request);
        }

        private LinkResponse GetLinkResponse(IRestResponse response)
        {
            if (response == null)
            {
                return null;
            }

            return new LinkResponse
            {
                StatusCode = (int?) response.StatusCode
            };
        }

        private async Task<LinkResponse> GetAndSaveResponseAsync(string href)
        {
            var response = await GetResponseFromRedisAsync(href);

            if (response == null)
            {
                var client = new RestClient($"http://{href}") { Timeout = -1 };
                var request = new RestRequest(Method.GET);
                response = GetLinkResponse(await client.ExecuteAsync(request));
                await SetResponseToRedisAsync(href, response);
            }

            Console.WriteLine($"Полученый response={response}");
            return response;
        }

        private async Task<LinkResponse> GetResponseFromRedisAsync(string href)
        {
            var encodedResponse = await _distributedCache.GetAsync(href);
            if (encodedResponse == null)
            {
                Console.WriteLine($"Объект в Redis не найден: {href}");
                return null;
            }

            var serializedResponse = Encoding.UTF8.GetString(encodedResponse);
            return JsonConvert.DeserializeObject<LinkResponse>(serializedResponse);
        }

        private async Task SetResponseToRedisAsync(string href, LinkResponse response)
        {
            var serializedResponse = JsonConvert.SerializeObject(response);
            var encodedResponse = Encoding.UTF8.GetBytes(serializedResponse);
            var options = new DistributedCacheEntryOptions()
               .SetSlidingExpiration(TimeSpan.FromMinutes(5))
               .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
            await _distributedCache.SetAsync(href, encodedResponse, options);

            Console.WriteLine($"Объект {href} с response={response} сохранен в Redis");
        }

        private class LinkStepBody
        {
            public LinkResponse Response { get; set; }
        }

        private class LinkResponse
        {
            public int? StatusCode { get; set; }

            public override string ToString()
            {
                return $"[statusCode={StatusCode}]";
            }
        }
    }
}