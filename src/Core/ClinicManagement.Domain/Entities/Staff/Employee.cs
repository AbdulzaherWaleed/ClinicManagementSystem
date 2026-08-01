using ClinicManagement.Domain.Common;

namespace ClinicManagement.Domain.Entities.Staff;

/// <summary>
/// Represents a clinic employee (receptionist, coordinator, etc.)
/// who can be assigned to one or more doctors (Many-to-Many via EmployeeDoctorAssignment).
/// </summary>
public class Employee : BaseAuditableEntity
{
    public string FullName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }

    public Guid ApplicationUserId { get; set; }

    public bool IsActive { get; set; } = true;

    // Many-to-Many: one employee can be assigned to multiple doctors
    public ICollection<EmployeeDoctorAssignment> DoctorAssignments { get; set; } = new List<EmployeeDoctorAssignment>();
}