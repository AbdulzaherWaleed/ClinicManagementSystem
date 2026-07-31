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

        builder.HasOne(e => e.AssignedDoctor)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.AssignedDoctorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(e => e.ApplicationUserId).IsUnique();

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}