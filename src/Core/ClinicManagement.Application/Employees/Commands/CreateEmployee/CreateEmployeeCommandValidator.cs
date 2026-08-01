using FluentValidation;

namespace ClinicManagement.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhoneNumber).MaximumLength(30).When(x => x.PhoneNumber != null);
        RuleFor(x => x.JobTitle).MaximumLength(100).When(x => x.JobTitle != null);
        RuleFor(x => x.ApplicationUserId).NotEmpty();
        RuleFor(x => x.AssignedDoctorIds)
            .NotEmpty()
            .WithMessage("At least one doctor assignment is required.");
    }
}
