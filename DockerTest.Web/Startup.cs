using System;
using Autofac;
using DockerTest.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            builder.RegisterModule(new DataModule(sqlConnectionString));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            InitializeDatabase(app.ApplicationServices);
        }

        private static void InitializeDatabase(IServiceProvider serviceProvider)
        {
            Seeder.Migrate(serviceProvider);
        }
    }
}