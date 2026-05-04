using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.VendorId).IsRequired();
        builder.Property(i => i.QuantityOnHand).IsRequired();
        builder.Property(i => i.QuantityReserved).IsRequired();
        builder.Property(i => i.LowStockThreshold).IsRequired();

        builder.Ignore(i => i.QuantityAvailable);
        builder.Ignore(i => i.IsLowStock);

        builder.HasIndex(i => i.ProductId).IsUnique();

        builder.ToTable("InventoryItems");
    }
}
