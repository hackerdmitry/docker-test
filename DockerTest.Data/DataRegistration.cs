using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DockerTest.Data
{
    public static class DataRegistration
    {
        public static IServiceCollection UseApplicationData(this IServiceCollection services, string sqlConnectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(sqlConnectionString,
                                                                                     b => b.MigrationsAssembly("DockerTest.Data")));

            return services;
        }
    }
}