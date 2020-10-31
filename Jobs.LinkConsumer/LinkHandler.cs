using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DockerTest.Data.Entities;
using DockerTest.Data.Events;
using DockerTest.Data.Infrastructure.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;

namespace Jobs.LinkConsumer
{
    public class LinkHandler
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly IRepository<Link> _linkRepository;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public LinkHandler(RabbitMqConfiguration configuration,
                           IRepository<Link> linkRepository,
                           IUnitOfWorkFactory unitOfWorkFactory)
        {
            _configuration = configuration;
            _linkRepository = linkRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
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
                channel.QueueDeclare(queue: _configuration.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

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
            var link = _linkRepository.GetAll().FirstOrDefault(x => x.Id == @event.Id);
            if (link == null)
            {
                throw new Exception($"Not found event with id={@event.Id}");
            }

            using (var uow = _unitOfWorkFactory.GetUoW())
            {
                link.LinkStatus = LinkStatus.Processing;
                uow.Commit();
            }

            for (var i = link.CurrentStep; i < link.CountStep; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(link.Tact));
                using (var uow = _unitOfWorkFactory.GetUoW())
                {
                    link.CurrentStep = i + 1;
                    uow.Commit();
                }
            }

            using (var uow = _unitOfWorkFactory.GetUoW())
            {
                var client = new RestClient($"http://{link.Href}") {Timeout = -1};
                var request = new RestRequest(Method.GET);
                var response = await client.ExecuteAsync(request);
                link.Status = (int?) response.StatusCode;
                link.CurrentStep = link.CountStep;
                link.LinkStatus = LinkStatus.Done;
                uow.Commit();
            }
        }
    }
}