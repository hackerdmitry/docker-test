using Microsoft.EntityFrameworkCore;

namespace DockerTest.Data.Infrastructure.Interfaces
{
    public interface IEfContextManager<out TDbContext> where TDbContext : DbContext
    {
        TDbContext CreateContext();

        TDbContext GetContext();
    }
}