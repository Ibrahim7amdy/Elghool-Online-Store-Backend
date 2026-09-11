using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasKey(a => a.Admin_Id);

        builder.Property(a => a.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.PasswordHash)
            .IsRequired();

        builder.HasIndex(a => a.Email).IsUnique();

    }
}