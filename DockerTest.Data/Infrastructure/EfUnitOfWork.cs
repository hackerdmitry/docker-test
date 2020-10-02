using System;
using System.Threading;
using System.Threading.Tasks;
using DockerTest.Data.Infrastructure.Interfaces;

namespace DockerTest.Data.Infrastructure
{
    public sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public EfUnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }

        public void Dispose() { }
    }
}