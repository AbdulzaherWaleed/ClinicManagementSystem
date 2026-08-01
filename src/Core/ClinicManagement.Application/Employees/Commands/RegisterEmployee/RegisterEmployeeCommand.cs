using MediatR;

namespace ClinicManagement.Application.Employees.Commands.RegisterEmployee;

public class RegisterEmployeeCommand : IRequest<Guid>
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }

    /// <summary>
    /// IDs of doctors this employee is assigned to.
    /// </summary>
    public List<Guid> AssignedDoctorIds { get; set; } = new();
}
