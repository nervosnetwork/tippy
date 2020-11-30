using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;

namespace Tippy.Core.Data
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .Property(p => p.Chain)
                .HasConversion<string>();

            modelBuilder.Entity<Project>()
                .Property(p => p.IsActive)
                .HasDefaultValue(false);
        }
    }
}
