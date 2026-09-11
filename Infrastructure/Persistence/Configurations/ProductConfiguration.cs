using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.PurchasePrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");


        builder.Property(p => p.DiscountPercentage)
            .HasColumnType("decimal(5,2)");

        builder.Property(p => p.UnitType)
        .HasConversion<string>()
        .HasMaxLength(30)
        .IsRequired();

        builder.Property(p => p.Weight)
            .HasColumnType("decimal(10,2)");

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.OrderItems)
               .WithOne(o => o.Product)
               .HasForeignKey(o => o.ProductId);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);
    }
}