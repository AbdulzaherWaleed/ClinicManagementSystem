using ClinicManagement.Domain.Common;

namespace ClinicManagement.Domain.Entities.Settings;

public class ClinicSetting : BaseAuditableEntity
{
    public Guid? DoctorId { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
}