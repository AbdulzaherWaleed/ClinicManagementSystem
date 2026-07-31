using ClinicManagement.Domain.Entities.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.NationalId).HasMaxLength(50);
        builder.Property(p => p.PhoneNumber).HasMaxLength(30);

        builder.HasIndex(p => p.NationalId);
        builder.HasIndex(p => p.PhoneNumber);
        builder.HasIndex(p => p.FullName);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}