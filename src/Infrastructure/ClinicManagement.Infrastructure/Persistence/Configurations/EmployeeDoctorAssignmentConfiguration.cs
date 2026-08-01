using ClinicManagement.Domain.Entities.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the EmployeeDoctorAssignment join entity.
/// Maps the Many-to-Many relationship between Employee and Doctor.
/// </summary>
public class EmployeeDoctorAssignmentConfiguration : IEntityTypeConfiguration<EmployeeDoctorAssignment>
{
    public void Configure(EntityTypeBuilder<EmployeeDoctorAssignment> builder)
    {
        builder.ToTable("EmployeeDoctorAssignments");

        // Composite unique index to prevent duplicate assignments
        builder.HasIndex(e => new { e.EmployeeId, e.DoctorId }).IsUnique();

        builder.HasOne(a => a.Employee)
            .WithMany(e => e.DoctorAssignments)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.EmployeeAssignments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
