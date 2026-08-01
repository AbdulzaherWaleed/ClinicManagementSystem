using ClinicManagement.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

        // v2.1 — AssignedDoctorIdsRaw stores comma-separated Guids (e.g. "guid1,guid2")
        // The AssignedDoctorIds computed property is NOT mapped to DB (it's derived).
        builder.Property(u => u.AssignedDoctorIdsRaw).HasMaxLength(2000);
        builder.Ignore(u => u.AssignedDoctorIds);

        // Removed: old single AssignedDoctorId FK nav property (replaced by AssignedDoctorIdsRaw)
    }
}