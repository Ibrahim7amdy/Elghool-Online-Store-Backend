using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {

        builder.HasOne(st => st.SourceBranch)
            .WithMany()
            .HasForeignKey(st => st.SourceBranchId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(st => st.DestinationBranch)
            .WithMany()
            .HasForeignKey(st => st.DestinationBranchId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(st => st.Product)
            .WithMany()
            .HasForeignKey(st => st.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}