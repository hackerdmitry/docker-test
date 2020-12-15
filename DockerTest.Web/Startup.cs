using System;
using System.Linq;
using Autofac;
using DockerTest.Data;
using DockerTest.Data.Entities;
using DockerTest.Data.Events;
using DockerTest.Data.Infrastructure.Interfaces;
using DockerTest.Web.Configurations;
using DockerTest.Web.Services;
using DockerTest.Web.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DockerTest.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
               .AddRazorRuntimeCompilation();

            services.AddRouting(options => options.LowercaseUrls = true);

            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.UseApplicationData(sqlConnectionString);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "docker-test API", Version = "v1" });
                c.DocumentFilter<ApiDocumentFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            var rabbitMqSettings = Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

            builder.RegisterModule(new DataModule(sqlConnectionString));
            builder.RegisterInstance(rabbitMqSettings);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "docker-test API"));

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("apiRoute", "api/{controller}/{action}");
                endpoints.MapControllerRoute("apiControllerRoute", "api/{controller}");
                endpoints.MapControllerRoute("apiActionRoute", "api/");
                endpoints.MapControllerRoute("defaults", "{controller}/{action}");
            });

            InitializeDatabase(app.ApplicationServices);
            RefreshRabbitMqQueue(app.ApplicationServices);
        }

        private static void InitializeDatabase(IServiceProvider serviceProvider)
        {
            Seeder.Migrate(serviceProvider);
        }

        private static void RefreshRabbitMqQueue(IServiceProvider serviceProvider)
        {
            var linkRepository = serviceProvider.GetService<IRepository<Link>>();
            var unitOfWorkFactory = serviceProvider.GetService<IUnitOfWorkFactory>();
            var rabbitMqConfiguration = serviceProvider.GetService<RabbitMqConfiguration>();
            var rabbitMqService = new RabbitMqService(rabbitMqConfiguration);

            using (var uow = unitOfWorkFactory.GetUoW())
            {
                var links = linkRepository.GetAll()
                   .Where(x => x.LinkStatus == LinkStatus.Waiting)
                   .ToArray();

                foreach (var link in links)
                {
                    var linkEvent = new LinkEvent { Id = link.Id, Href = link.Href };
                    var successfullySent = rabbitMqService.SendLinkEvent(linkEvent);
                    link.LinkStatus = successfullySent ? LinkStatus.Queue : LinkStatus.Waiting;
                }

                uow.Commit();
            }
        }
    }
}