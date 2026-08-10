using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (employee is null)
            throw new NotFoundException(nameof(Domain.Entities.Staff.Employee), request.Id);

        // Apply only explicitly provided scalar fields (PATCH semantics)
        if (request.FullName != null)    employee.FullName    = request.FullName;
        if (request.PhoneNumber != null) employee.PhoneNumber = request.PhoneNumber;
        if (request.JobTitle != null)    employee.JobTitle    = request.JobTitle;
        if (request.IsActive.HasValue)   employee.IsActive    = request.IsActive.Value;

        // Collection semantics (stated in UpdateEmployeeCommand summary):
        //   null        → don't touch existing assignments
        //   []          → clear all assignments
        //   [ids]       → full replace
        if (request.AssignedDoctorIds != null)
        {
            var existing = await _context.EmployeeDoctorAssignments
                .Where(a => a.EmployeeId == employee.Id && !a.IsDeleted)
                .ToListAsync(cancellationToken);

            // Soft-delete all current (interceptor sets IsDeleted=true automatically)
            foreach (var assignment in existing)
                _context.EmployeeDoctorAssignments.Remove(assignment);

            // Add the new set (may be empty = "clear all")
            foreach (var doctorId in request.AssignedDoctorIds)
            {
                _context.EmployeeDoctorAssignments.Add(new EmployeeDoctorAssignment
                {
                    EmployeeId = employee.Id,
                    DoctorId   = doctorId
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
