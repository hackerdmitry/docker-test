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
            var rabbitMqSettings = configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();
            var globalSettings = configuration.GetSection("GlobalSettings").Get<GlobalSettings>();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(rabbitMqSettings);
            builder.RegisterInstance(globalSettings);
            builder.RegisterType<LinkHandler>();

            var container = builder.Build();
            return container;
        }
    }
}