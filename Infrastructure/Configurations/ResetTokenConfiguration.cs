using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ResetTokenConfiguration : IEntityTypeConfiguration<ResetToken>
{
    public void Configure(EntityTypeBuilder<ResetToken> builder)
    {
        // Table
        builder.ToTable("ResetTokens");

        // Key
        builder.HasKey(rt => rt.Id);
        
        // Properties
        builder.Property(rt => rt.Token).IsRequired().HasMaxLength(6);
        builder.Property(rt => rt.ExpiresOnUtc).IsRequired();
        builder.Property(rt => rt.UserId).IsRequired();

        // Relations
        builder.HasOne(rt => rt.User)
            .WithOne()
            .HasForeignKey<ResetToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(rt => rt.UserId);
        builder.HasIndex(rt => rt.Token);
    }
}
