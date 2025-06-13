using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.ToTable("PromoCodes");

            // Key
            builder.HasKey(pc => pc.Id);

            // Properties
            builder.Property(pc => pc.Code).IsRequired().HasMaxLength(40);
            builder.Property(pc => pc.DiscountValue).IsRequired();
            builder.Property(pc => pc.ExpiryDate).IsRequired();
            builder.Property(pc => pc.IsActive).IsRequired();

            // Indexes
            builder.HasIndex(pc => pc.Code);
        }
    }
}
