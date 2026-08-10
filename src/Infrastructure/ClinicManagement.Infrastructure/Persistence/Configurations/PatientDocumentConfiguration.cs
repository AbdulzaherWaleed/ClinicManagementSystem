using ClinicManagement.Domain.Entities.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class PatientDocumentConfiguration : IEntityTypeConfiguration<PatientDocument>
{
    public void Configure(EntityTypeBuilder<PatientDocument> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.FileType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.Documents)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict); // Strict requirement: deleting a patient doesn't cascade-delete historical docs

        builder.HasIndex(x => new { x.PatientId, x.CreatedAt }); // Added for pagination

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
