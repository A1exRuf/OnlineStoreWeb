using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        // Key
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Email).IsRequired().HasMaxLength(254);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(254);
        builder.Property(u => u.CreatedAt).IsRequired();

        // Relations
        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId);

        // Indexes
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
