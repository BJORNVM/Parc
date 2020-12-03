using Microsoft.EntityFrameworkCore;
using Parc.Models;

namespace Parc.Data
{
    public class ParcDbContext : DbContext
    {
        public ParcDbContext(DbContextOptions<ParcDbContext> options) : base(options)
        {

        }

        public DbSet<ParcEvent> Events { get; set; }
    }
}
