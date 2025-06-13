using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        // Table
        builder.ToTable("ProductImages");

        // Key
        builder.HasKey(pi => pi.Id);

        // Properties
        builder.Property(pi => pi.ProductId).IsRequired();
        builder.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(50);
        builder.Property(pi => pi.AltText).HasMaxLength(20);
        builder.Property(pi => pi.DisplayOrder).IsRequired();

        // Relations
        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(pi => pi.ProductId);
    }
}
