using Autofac;
using DockerTest.Data;
using Jobs.Core;
using Microsoft.Extensions.Configuration;

namespace Jobs.LinkConsumer
{
    public class Startup : BaseConsoleStartup
    {
        protected override IContainer ConfigureServices(IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var rabbitMqSettings = configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(rabbitMqSettings);
            builder.RegisterModule(new DataModule(connectionString));
            builder.RegisterType<LinkHandler>();

            var container = builder.Build();
            return container;
        }
    }
}