using ClinicManagement.Domain.Entities.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class DoctorLicenseConfiguration : IEntityTypeConfiguration<DoctorLicense>
{
    public void Configure(EntityTypeBuilder<DoctorLicense> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LicenseNumber)
            .HasMaxLength(100);

        builder.Property(x => x.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FileType)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.Licenses)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
