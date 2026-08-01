using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Employees.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, List<EmployeeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllEmployeesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .Include(e => e.DoctorAssignments)
                .ThenInclude(da => da.Doctor)
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
            })
            .ToListAsync(cancellationToken);
    }
}
