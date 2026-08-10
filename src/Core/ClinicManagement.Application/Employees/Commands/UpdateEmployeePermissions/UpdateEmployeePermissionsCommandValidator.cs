using ClinicManagement.Domain.Constants;
using FluentValidation;

namespace ClinicManagement.Application.Employees.Commands.UpdateEmployeePermissions;

public class UpdateEmployeePermissionsCommandValidator : AbstractValidator<UpdateEmployeePermissionsCommand>
{
    public UpdateEmployeePermissionsCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required.");

        RuleForEach(x => x.Permissions)
            .Must(p => AppPermissions.GetAll().Contains(p))
            .WithMessage("One or more permissions provided are invalid.");
    }
}
