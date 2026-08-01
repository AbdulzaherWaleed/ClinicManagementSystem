using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Entities.Doctors;

namespace ClinicManagement.Domain.Entities.Staff;

/// <summary>
/// Join entity representing the Many-to-Many relationship
/// between an Employee and the Doctors they are assigned to.
/// Managed exclusively by Admin at Employee Create/Edit time.
/// </summary>
public class EmployeeDoctorAssignment : BaseAuditableEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;
}
