using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BranchInventoryConfiguration : IEntityTypeConfiguration<BranchInventory>
{
    public void Configure(EntityTypeBuilder<BranchInventory> builder)
    {
        builder.HasKey(bi => new { bi.BranchId, bi.ProductId });

        builder.HasOne(bi => bi.Product)
            .WithMany(p => p.BranchInventories)
            .HasForeignKey(bi => bi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(bi => bi.Branch)
            .WithMany(b => b.BranchInventories)
            .HasForeignKey(bi => bi.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(bi => bi.LowStockThreshold)
            .HasDefaultValue(10);
    }
}