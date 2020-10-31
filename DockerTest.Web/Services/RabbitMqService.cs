using System;
using System.Text;
using DockerTest.Data.Events;
using DockerTest.Web.Configurations;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace DockerTest.Web.Services
{
    public class RabbitMqService
    {
        private readonly RabbitMqConfiguration _configuration;

        public RabbitMqService(RabbitMqConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendLinkEvent(LinkEvent linkEvent)
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

                var message = JsonConvert.SerializeObject(linkEvent);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                    routingKey: _configuration.QueueName,
                    basicProperties: properties,
                    body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}