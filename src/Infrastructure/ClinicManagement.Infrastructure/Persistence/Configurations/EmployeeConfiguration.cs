using ClinicManagement.Domain.Entities.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.Property(e => e.FullName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.PhoneNumber).HasMaxLength(30);
        builder.Property(e => e.JobTitle).HasMaxLength(100);

        // The M2M relationship is configured in EmployeeDoctorAssignmentConfiguration
        // No HasOne/HasForeignKey for AssignedDoctorId anymore (v2.1)

        builder.HasIndex(e => e.ApplicationUserId).IsUnique();

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}