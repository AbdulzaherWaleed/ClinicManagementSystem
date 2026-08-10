using FluentValidation;

namespace ClinicManagement.Application.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Doctor ID is required.");

        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("Full name cannot be empty.")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters.")
            .When(v => v.FullName != null);

        RuleFor(v => v.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.")
            .When(v => v.Email != null);

        RuleFor(v => v.PhoneNumber)
            .Matches(@"^[\+\d\s\-\(\)]{7,30}$").WithMessage("Invalid phone number format.")
            .When(v => v.PhoneNumber != null);

        RuleFor(v => v.Title)
            .MaximumLength(50).WithMessage("Title cannot exceed 50 characters.")
            .When(v => v.Title != null);
    }
}
