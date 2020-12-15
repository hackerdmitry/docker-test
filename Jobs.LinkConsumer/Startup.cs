using Autofac;
using Autofac.Extensions.DependencyInjection;
using Jobs.Core;
using Jobs.LinkConsumer.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jobs.LinkConsumer
{
    public class Startup : BaseConsoleStartup
    {
        protected override IContainer ConfigureServices(IConfigurationRoot configuration)
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();
            var globalSettings = configuration.GetSection("GlobalSettings").Get<GlobalSettings>();
            var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(rabbitMqSettings);
            builder.RegisterInstance(globalSettings);
            builder.RegisterInstance(redisSettings);
            builder.RegisterType<LinkHandler>();

            var services = new ServiceCollection();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{redisSettings.Host}:{redisSettings.Port}";

            });
            builder.Populate(services);

            var container = builder.Build();
            return container;
        }
    }
}