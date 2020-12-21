using Microsoft.EntityFrameworkCore;
using Parc.Models;

namespace Parc.Data
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {

        }

        public DbSet<ImportResult> ImportResults { get; set; }
    }
}
