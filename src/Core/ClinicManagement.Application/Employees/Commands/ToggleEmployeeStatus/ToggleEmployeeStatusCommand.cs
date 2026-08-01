using MediatR;

namespace ClinicManagement.Application.Employees.Commands.ToggleEmployeeStatus;

public class ToggleEmployeeStatusCommand : IRequest
{
    public Guid Id { get; set; }
}
