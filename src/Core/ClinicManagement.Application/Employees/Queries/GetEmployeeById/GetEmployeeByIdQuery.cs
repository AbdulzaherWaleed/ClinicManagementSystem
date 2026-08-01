using ClinicManagement.Application.Employees.DTOs;
using MediatR;

namespace ClinicManagement.Application.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQuery : IRequest<EmployeeDto?>
{
    public Guid Id { get; set; }
}
