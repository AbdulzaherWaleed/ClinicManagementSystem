using MediatR;

namespace ClinicManagement.Application.Employees.Queries.GetEmployeePermissions;

public class GetEmployeePermissionsQuery : IRequest<List<string>>
{
    public Guid EmployeeId { get; set; }
}
