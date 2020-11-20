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

        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .Property(c => c.Chain)
                .HasConversion<string>();
        }
    }
}
