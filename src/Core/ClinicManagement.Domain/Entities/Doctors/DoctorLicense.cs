using ClinicManagement.Domain.Common;

namespace ClinicManagement.Domain.Entities.Doctors;

public class DoctorLicense : BaseAuditableEntity
{
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    public string? LicenseNumber { get; set; }
    
    public string FilePath { get; set; } = default!;
    
    public string OriginalFileName { get; set; } = default!;

    public string FileType { get; set; } = default!;
    
    public DateTime? ExpiryDate { get; set; }
    
    public bool IsActive { get; set; } = true;
}
