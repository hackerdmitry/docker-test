using System.Linq;
using DockerTest.Data.Infrastructure.Interfaces;

namespace DockerTest.Data.Infrastructure
{
    public class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly ApplicationDbContext Context;

        public EfRepository(IEfContextManager<ApplicationDbContext> contextManager)
        {
            Context = contextManager.GetContext();
        }

        public IQueryable<TEntity> GetAll()
        {
            return Context.Set<TEntity>();
        }

        public TEntity Add(TEntity entity)
        {
            return Context.Set<TEntity>().Add(entity).Entity;
        }

        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }
    }
}