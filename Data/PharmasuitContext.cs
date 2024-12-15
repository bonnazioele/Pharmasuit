using Microsoft.EntityFrameworkCore;
using Pharmasuit.Models;


namespace Pharmasuit.Data
{
    public class PharmasuitContext : DbContext
    {
        public PharmasuitContext(DbContextOptions<PharmasuitContext> options ) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Pharmasuit.Models.Account> Account { get; set; } = default!;
        public DbSet<Pharmasuit.Models.OrderDetails> OrderDetails { get; set; } = default!;
        public DbSet<Pharmasuit.Models.Cart> Cart { get; set; } = default!;

    }
}
