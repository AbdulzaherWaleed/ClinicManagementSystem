using ClinicManagement.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);

        // v2.1 — Visit Stage stored as string for readability in DB
        builder.Property(a => a.VisitStage).HasConversion<string>().HasMaxLength(20);

        // v2.1 — Visit Type / Specialty (free text: Botox, ENT, Ophthalmology, etc.)
        builder.Property(a => a.VisitType).HasMaxLength(100);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.DoctorId, a.ScheduledStart });

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}