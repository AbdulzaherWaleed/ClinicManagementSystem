using FluentValidation;

namespace ClinicManagement.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Employee ID is required.");

        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("Full name cannot be empty.")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters.")
            .When(v => v.FullName != null);

        RuleFor(v => v.PhoneNumber)
            .Matches(@"^[\+\d\s\-\(\)]{7,30}$").WithMessage("Invalid phone number format.")
            .When(v => v.PhoneNumber != null);

        RuleFor(v => v.JobTitle)
            .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters.")
            .When(v => v.JobTitle != null);

        RuleForEach(v => v.AssignedDoctorIds)
            .NotEmpty().WithMessage("Doctor ID in assignment list cannot be empty.")
            .When(v => v.AssignedDoctorIds != null && v.AssignedDoctorIds.Count > 0);
    }
}
