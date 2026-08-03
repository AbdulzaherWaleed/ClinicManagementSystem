using ClinicManagement.Application.Employees.DTOs;
using MediatR;

using ClinicManagement.Application.Common.Models;

namespace ClinicManagement.Application.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQuery : IRequest<PaginatedList<EmployeeDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
