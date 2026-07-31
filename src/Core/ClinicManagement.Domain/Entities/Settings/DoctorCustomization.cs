using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Entities.Doctors;

namespace ClinicManagement.Domain.Entities.Settings;

public class DoctorCustomization : BaseAuditableEntity
{
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    public string? LogoStoragePath { get; set; }
    public string? PrimaryColorHex { get; set; }
    public string? SecondaryColorHex { get; set; }

    public string? ReportHeaderText { get; set; }
    public string? ReportFooterText { get; set; }
    public string? ClinicDisplayName { get; set; }

    public string? InvoicePrefix { get; set; }

    public string? ExtraSettingsJson { get; set; }
}