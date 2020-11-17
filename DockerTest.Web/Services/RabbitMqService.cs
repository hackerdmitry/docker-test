using System;
using System.Collections.Generic;
using System.Text;
using DockerTest.Data.Events;
using DockerTest.Web.Configurations;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace DockerTest.Web.Services
{
    public class RabbitMqService
    {
        private static HashSet<string> declaredQueues = new HashSet<string>();
        private readonly RabbitMqConfiguration _configuration;

        public RabbitMqService(RabbitMqConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory
            {
                HostName = _configuration.Host,
                UserName = _configuration.Username,
                Password = _configuration.Password
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                QueueDeclare(channel, _configuration.LinkQueueName);
            }
        }

        public bool SendLinkEvent(LinkEvent linkEvent)
        {
            var success = Publish("link_queue", linkEvent, 5);
            return success;
        }

        private bool Publish(string queueName, object messageObject, int messageLength = -1)
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
                QueueDeclare(channel, queueName);

                if (messageLength != -1 && channel.MessageCount(queueName) < messageLength)
                {
                    var message = JsonConvert.SerializeObject(messageObject);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                        routingKey: queueName,
                        basicProperties: properties,
                        body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                    return true;
                }
            }

            return false;
        }

        private void QueueDeclare(IModel channel,
                                  string queueName,
                                  bool durable = true,
                                  bool exclusive = false,
                                  bool autoDelete = false,
                                  Dictionary<string, object> arguments = null)
        {
            if (!declaredQueues.Contains(queueName))
            {
                channel.QueueDelete(queueName);
                channel.QueueDeclare(queueName,
                    durable,
                    exclusive,
                    autoDelete,
                    arguments);
                declaredQueues.Add(queueName);
            }
        }
    }
}