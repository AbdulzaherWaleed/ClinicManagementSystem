using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Identity;
using ClinicManagement.Domain.Entities.Staff;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagement.Application.Employees.Commands.RegisterEmployee;

public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, Guid>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IApplicationDbContext _context;

    public RegisterEmployeeCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<Guid> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate email uniqueness
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("Email", "البريد الإلكتروني مسجل مسبقاً.")
            });
        }

        // 2. Ensure Employee Role exists
        var roleName = UserRole.Employee.ToString();
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

        // 3. Create Identity User
        var assignedDoctorIdsRaw = request.AssignedDoctorIds.Count > 0
            ? string.Join(",", request.AssignedDoctorIds)
            : null;

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = UserRole.Employee,
            AssignedDoctorIdsRaw = assignedDoctorIdsRaw,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.Select(e =>
                new FluentValidation.Results.ValidationFailure(string.Empty, e.Description)));
        }

        await _userManager.AddToRoleAsync(user, roleName);

        // 4. Create Domain Entity
        var employee = new Employee
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            JobTitle = request.JobTitle,
            ApplicationUserId = user.Id,
            IsActive = true
        };

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
