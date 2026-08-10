using FluentValidation;

namespace ClinicManagement.Application.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Patient ID is required.");

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

        RuleFor(v => v.Gender)
            .Must(g => g == "Male" || g == "Female" || g == "Other")
            .WithMessage("Gender must be Male, Female, or Other.")
            .When(v => v.Gender != null);

        RuleFor(v => v.NationalId)
            .MaximumLength(50).WithMessage("National ID cannot exceed 50 characters.")
            .When(v => v.NationalId != null);

        RuleFor(v => v.EmergencyContactPhone)
            .Matches(@"^[\+\d\s\-\(\)]{7,30}$").WithMessage("Invalid emergency contact phone format.")
            .When(v => v.EmergencyContactPhone != null);
    }
}
