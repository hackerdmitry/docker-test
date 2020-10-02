using DockerTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DockerTest.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Note> Notes { get; set; }
    }
}