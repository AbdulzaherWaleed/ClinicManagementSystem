using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Entities.Patients;

namespace ClinicManagement.Domain.Entities.Patients;

public class PatientDocument : BaseAuditableEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public string DocumentType { get; set; } = default!; // Prescription | LabResult | XRay | Other
    public string OriginalFileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public string FileType { get; set; } = default!; // PDF | Image
    public string? Notes { get; set; }
}
