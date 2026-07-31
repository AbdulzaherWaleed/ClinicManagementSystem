using ClinicManagement.Domain.Entities.Doctors;
using ClinicManagement.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.Property(d => d.FullName).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Title).HasMaxLength(50);
        builder.Property(d => d.Email).HasMaxLength(200);
        builder.Property(d => d.PhoneNumber).HasMaxLength(30);

        builder.HasIndex(d => d.Email);

        builder.HasOne(d => d.PrimarySpecialty)
            .WithMany(s => s.Doctors)
            .HasForeignKey(d => d.PrimarySpecialtyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(d => d.Appointments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.PatientVisits)
            .WithOne(v => v.Doctor)
            .HasForeignKey(v => v.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Employees)
            .WithOne(e => e.AssignedDoctor)
            .HasForeignKey(e => e.AssignedDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Documents)
            .WithOne(doc => doc.Doctor)
            .HasForeignKey(doc => doc.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Customization)
            .WithOne(c => c.Doctor)
            .HasForeignKey<DoctorCustomization>(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}