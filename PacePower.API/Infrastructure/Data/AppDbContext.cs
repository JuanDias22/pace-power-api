using Microsoft.EntityFrameworkCore;

namespace PacePower.API.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lead> Leads { get; set; }
    }
}