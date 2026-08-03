using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Employees.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

using ClinicManagement.Application.Common.Models;

namespace ClinicManagement.Application.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, PaginatedList<EmployeeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllEmployeesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderBy(e => e.FullName)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                PhoneNumber = e.PhoneNumber,
                JobTitle = e.JobTitle,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                AssignedDoctorIds = e.DoctorAssignments
                    .Where(da => !da.IsDeleted)
                    .Select(da => da.DoctorId)
                    .ToList(),
                AssignedDoctorNames = e.DoctorAssignments
                    .Where(da => !da.IsDeleted)
                    .Select(da => da.Doctor.FullName)
                    .ToList()
            });

        return await PaginatedList<EmployeeDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
    }
}
