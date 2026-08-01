using MediatR;

namespace ClinicManagement.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommand : IRequest<Guid>
{
    public string FullName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }

    /// <summary>
    /// The application user account (pre-created via Auth/Register) to link to this employee.
    /// </summary>
    public Guid ApplicationUserId { get; set; }

    /// <summary>
    /// v2.1 — IDs of doctors this employee is assigned to. At least 1 required.
    /// Admin-only field set at creation time.
    /// </summary>
    public List<Guid> AssignedDoctorIds { get; set; } = new();
}
