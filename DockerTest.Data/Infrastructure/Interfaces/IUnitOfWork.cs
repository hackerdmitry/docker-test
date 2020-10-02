using System;
using System.Threading;
using System.Threading.Tasks;

namespace DockerTest.Data.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        Task CommitAsync(CancellationToken ct);
    }
}