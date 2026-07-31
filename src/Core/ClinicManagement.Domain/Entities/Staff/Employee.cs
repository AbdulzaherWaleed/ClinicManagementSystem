using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Entities.Doctors;

namespace ClinicManagement.Domain.Entities.Staff;

public class Employee : BaseAuditableEntity
{
    public string FullName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }

    public Guid ApplicationUserId { get; set; }

    public Guid AssignedDoctorId { get; set; }
    public Doctor AssignedDoctor { get; set; } = default!;

    public bool IsActive { get; set; } = true;
}