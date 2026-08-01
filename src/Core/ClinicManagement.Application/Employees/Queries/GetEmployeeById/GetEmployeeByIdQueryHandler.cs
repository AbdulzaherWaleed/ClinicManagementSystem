using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Employees.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .Include(e => e.DoctorAssignments)
                .ThenInclude(da => da.Doctor)
            .Where(e => e.Id == request.Id && !e.IsDeleted)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
