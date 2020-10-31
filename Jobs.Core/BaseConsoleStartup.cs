using System;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Jobs.Core
{
    public class BaseConsoleStartup
    {
        public IContainer Initialize()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(AppContext.BaseDirectory))
               .AddJsonFile("appsettings.json", optional: true)
               .AddEnvironmentVariables();
            if (!string.IsNullOrWhiteSpace(environment))
            {
                Console.WriteLine("Environment: {0}", environment);
                if (environment == "Development")
                {
                    builder.AddJsonFile(
                        Path.Combine(AppContext.BaseDirectory, string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar),
                            $"appsettings.{environment}.json"),
                        optional: true);
                }
                else
                {
                    builder.AddJsonFile($"appsettings.{environment}.json", optional: true);
                }
            }

            var configuration = builder.Build();
            var container = ConfigureServices(configuration);
            return container;
        }

        protected virtual IContainer ConfigureServices(IConfigurationRoot configuration)
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();
            return container;
        }
    }
}