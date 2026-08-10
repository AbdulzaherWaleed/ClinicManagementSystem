using ClinicManagement.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class AppointmentStatusHistoryConfiguration : IEntityTypeConfiguration<AppointmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<AppointmentStatusHistory> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.AppointmentId, t.ChangedAt });

        builder.HasOne(t => t.Appointment)
            .WithMany()
            .HasForeignKey(t => t.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.ChangedByEmployee)
            .WithMany()
            .HasForeignKey(t => t.ChangedByEmployeeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
