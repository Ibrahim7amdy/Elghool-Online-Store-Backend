using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Customer_Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.PasswordHash)
            .IsRequired();

        builder.HasIndex(c => c.Email).IsUnique();
        builder.HasIndex(c => c.PhoneNumber).IsUnique();

        builder.Property(c => c.PreferredBranchId)
        .HasColumnName("PreferredBranchId");

        builder.HasOne(c => c.PreferredBranch)
            .WithMany()
            .HasForeignKey(c => c.PreferredBranchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);
    }
}