using MercadoPago;
using Microsoft.EntityFrameworkCore;
using PacePower.API.Domain.Entities;

namespace PacePower.API.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lead> Leads { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}