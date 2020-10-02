using DockerTest.Data.Infrastructure.Interfaces;

namespace DockerTest.Data.Infrastructure
{
    public class EfUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ApplicationDbContext _context;

        public EfUnitOfWorkFactory(IEfContextManager<ApplicationDbContext> contextManager)
        {
            _context = contextManager.GetContext();
        }

        public IUnitOfWork GetUoW()
        {
            return new EfUnitOfWork(_context);
        }
    }
}