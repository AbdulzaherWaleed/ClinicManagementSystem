using FluentValidation;
namespace ClinicManagement.Application.Employees.Commands.ToggleEmployeeStatus;
public class ToggleEmployeeStatusCommandValidator : AbstractValidator<ToggleEmployeeStatusCommand>
{
    public ToggleEmployeeStatusCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Employee ID is required.");
    }
}
