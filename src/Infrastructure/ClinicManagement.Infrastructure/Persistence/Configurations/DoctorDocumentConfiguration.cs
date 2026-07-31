using ClinicManagement.Domain.Entities.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class DoctorDocumentConfiguration : IEntityTypeConfiguration<DoctorDocument>
{
    public void Configure(EntityTypeBuilder<DoctorDocument> builder)
    {
        builder.ToTable("DoctorDocuments");

        builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
        builder.Property(d => d.StoragePath).IsRequired().HasMaxLength(500);
        builder.Property(d => d.ContentType).HasMaxLength(100);
        builder.Property(d => d.Type).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(d => d.ExpiryDate);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}