using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
{
    public void Configure(EntityTypeBuilder<StockReservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.OrderId).IsRequired();
        builder.Property(r => r.ProductId).IsRequired();
        builder.Property(r => r.Quantity).IsRequired();
        builder.Property(r => r.ExpiresAt).IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Ignore(r => r.IsExpired);

        builder.HasIndex(r => r.OrderId);
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.ExpiresAt);

        builder.ToTable("StockReservations");
    }
}
