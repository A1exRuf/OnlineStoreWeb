using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Table
        builder.ToTable("Orders");

        // Key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.UserId).IsRequired();
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.PaidDate);
        builder.Property(o => o.ShippingDate);
        builder.Property(o => o.CompletedDate);
        builder.Property(o => o.Address).IsRequired().HasMaxLength(160);

        // Relations
        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indeoes
        builder.HasIndex(o => o.UserId);
    }
}
