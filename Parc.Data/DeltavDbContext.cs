using Microsoft.EntityFrameworkCore;
using Parc.Models;

namespace Parc.Data
{
    public class DeltavDbContext : DbContext
    {
        public DeltavDbContext(DbContextOptions<DeltavDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define "Journal" as Events table in database
            // Define "Ord" as primary key instead of default "Id"
            modelBuilder.Entity<DeltaVEvent>()
                .ToTable("Journal")
                .HasKey(e => e.Ord);
        }

        public DbSet<DeltaVEvent> Events { get; set; }
    }
}
