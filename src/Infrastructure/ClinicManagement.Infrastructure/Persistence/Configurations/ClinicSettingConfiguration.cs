using ClinicManagement.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class ClinicSettingConfiguration : IEntityTypeConfiguration<ClinicSetting>
{
    public void Configure(EntityTypeBuilder<ClinicSetting> builder)
    {
        builder.ToTable("ClinicSettings");

        builder.Property(s => s.Key).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Value).IsRequired();

        builder.HasIndex(s => new { s.DoctorId, s.Key }).IsUnique();

        builder.HasOne<ClinicManagement.Domain.Entities.Doctors.Doctor>()
            .WithMany()
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}