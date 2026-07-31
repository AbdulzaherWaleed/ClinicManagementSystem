using ClinicManagement.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class DoctorCustomizationConfiguration : IEntityTypeConfiguration<DoctorCustomization>
{
    public void Configure(EntityTypeBuilder<DoctorCustomization> builder)
    {
        builder.ToTable("DoctorCustomizations");

        builder.Property(c => c.PrimaryColorHex).HasMaxLength(9);
        builder.Property(c => c.SecondaryColorHex).HasMaxLength(9);
        builder.Property(c => c.InvoicePrefix).HasMaxLength(20);

        builder.HasIndex(c => c.DoctorId).IsUnique();
    }
}