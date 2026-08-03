using FluentValidation;

namespace ClinicManagement.Application.Doctors.Commands.UploadDoctorLicense;

public class UploadDoctorLicenseCommandValidator : AbstractValidator<UploadDoctorLicenseCommand>
{
    public UploadDoctorLicenseCommandValidator()
    {
        RuleFor(v => v.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required.");

        RuleFor(v => v.LicenseNumber)
            .MaximumLength(100).WithMessage("License number must not exceed 100 characters.");
            
        RuleFor(v => v.ExpiryDate)
            .NotEmpty().WithMessage("Expiry Date is required.");

        RuleFor(v => v.OriginalFileName)
            .NotEmpty().WithMessage("File name is required.");
    }
}
