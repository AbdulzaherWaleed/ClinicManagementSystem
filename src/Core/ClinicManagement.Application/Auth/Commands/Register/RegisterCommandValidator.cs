using FluentValidation;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.Role).IsInEnum();

        // v2.1 — Employee must be assigned to at least one doctor (Many-to-Many)
        RuleFor(x => x.AssignedDoctorIds)
            .NotEmpty()
            .When(x => x.Role == UserRole.Employee)
            .WithMessage("Employees must be assigned to at least one doctor.");
    }
}