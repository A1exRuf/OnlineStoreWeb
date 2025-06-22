using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table
        builder.ToTable("Products");

        // Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(4000);
        builder.Property(p => p.Price).IsRequired();
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.CategoryId).IsRequired();
        builder.Property(p => p.PurchaseCount).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        
        // Relations
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Price);
        builder.HasIndex(p => p.StockQuantity);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.CreatedAt);
    }
}
