using System.Linq;

namespace DockerTest.Data.Infrastructure.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();

        TEntity Add(TEntity entity);

        void Remove(TEntity entity);
    }
}