using ClinicManagement.Application.Employees.DTOs;
using MediatR;

namespace ClinicManagement.Application.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQuery : IRequest<List<EmployeeDto>>
{
}
