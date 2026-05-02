using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(i => i.AltText)
            .HasMaxLength(500);

        builder.HasIndex(i => i.ProductId);

        builder.Ignore(i => i.DomainEvents);

        builder.ToTable("ProductImages");
    }
}
