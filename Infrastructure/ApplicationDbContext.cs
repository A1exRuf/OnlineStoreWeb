using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItem { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductImage> ProductImage { get; set; }
    public DbSet<PromoCode> PromoCode { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<User> User { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
