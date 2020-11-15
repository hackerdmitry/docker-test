using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jobs.LinkConsumer.Configurations;
using Jobs.LinkConsumer.Events;
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

        public LinkHandler(RabbitMqConfiguration configuration,
                           GlobalSettings globalSettings)
        {
            _configuration = configuration;
            _globalSettings = globalSettings;
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
                    try
                    {
                        await HandleLinkEventAsync(linkEvent);
                        Console.WriteLine(" [x] Done");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [!] Error");
                        Console.WriteLine(e);
                    }

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: _configuration.QueueName,
                    autoAck: false,
                    consumer: consumer);

                await Task.Delay(Timeout.Infinite);
            }
        }

        private async Task HandleLinkEventAsync(LinkEvent @event)
        {
            for (var i = 0; i < 5; i++)
            {
                var uri = $"{_globalSettings.PublicHost}/api/link/step?id={@event.Id}";
                Console.WriteLine($"Попытка #{i + 1}: {uri}");
                var client = new RestClient(uri) {Timeout = -1};
                var request = new RestRequest(Method.PUT);
                do
                {
                    var response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                    {
                        var tact = double.Parse(response.Content);
                        if (tact == -1)
                        {
                            goto taskSuccess;
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

            taskSuccess: ;
        }
    }
}