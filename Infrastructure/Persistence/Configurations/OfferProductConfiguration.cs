using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OfferProductConfiguration : IEntityTypeConfiguration<OfferProduct>
{
    public void Configure(EntityTypeBuilder<OfferProduct> builder)
    {
        builder.HasKey(op => new { op.OfferId, op.ProductId });

        builder.HasOne(op => op.Offer)
            .WithMany(o => o.OfferProducts)
            .HasForeignKey(op => op.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(op => op.Product)
            .WithMany(p => p.OfferProducts)
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}