using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        // Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId).IsRequired();

        // Indexes
        builder.HasIndex(x => x.UserId);
    }
}
