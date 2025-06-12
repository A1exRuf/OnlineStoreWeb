using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        // Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x =>  x.Name).IsRequired();
        builder.Property(x => x.ParentCategoryId);

        // Indexes
        builder.HasIndex(x => x.ParentCategoryId);
    }
}
