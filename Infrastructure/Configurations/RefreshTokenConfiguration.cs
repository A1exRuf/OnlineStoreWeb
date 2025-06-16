using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Table
        builder.ToTable("RefreshTokens");

        // Key
        builder.HasKey(rt => rt.Id);

        // Properties
        builder.Property(rt => rt.Token).IsRequired().HasMaxLength(200);
        builder.Property(rt => rt.ExpiresOnUtc).IsRequired();
        builder.Property(rt => rt.UserId).IsRequired();

        // Relations
        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(rt => rt.Token).IsUnique();
    }
}
