using Autofac;
using DockerTest.Data.Infrastructure;
using DockerTest.Data.Infrastructure.Interfaces;

namespace DockerTest.Data
{
    public class DataModule : Module
    {
        public DataModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly string _connectionString;

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EfRepository<>))
                   .As(typeof(IRepository<>));

            builder.RegisterType(typeof(EfUnitOfWork))
                   .As(typeof(IUnitOfWork));

            builder.RegisterType(typeof(EfUnitOfWorkFactory))
                   .As(typeof(IUnitOfWorkFactory));

            builder.RegisterType(typeof(ApplicationDbContext))
                   .As<ApplicationDbContext>()
                   .InstancePerLifetimeScope();

            builder.Register(x => new ApplicationContextManager(_connectionString))
                   .As<IEfContextManager<ApplicationDbContext>>()
                   .InstancePerLifetimeScope();
        }
    }
}