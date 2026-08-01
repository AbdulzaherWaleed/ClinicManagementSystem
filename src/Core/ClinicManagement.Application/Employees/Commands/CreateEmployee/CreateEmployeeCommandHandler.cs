using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Staff;
using MediatR;

namespace ClinicManagement.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            JobTitle = request.JobTitle,
            ApplicationUserId = request.ApplicationUserId,
            IsActive = true
        };

        // v2.1 — Add M2M assignments (one EmployeeDoctorAssignment per doctor)
        foreach (var doctorId in request.AssignedDoctorIds)
        {
            employee.DoctorAssignments.Add(new EmployeeDoctorAssignment
            {
                DoctorId = doctorId
            });
        }

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}
