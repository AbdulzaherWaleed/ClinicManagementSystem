using FluentValidation;
namespace ClinicManagement.Application.Doctors.Commands.ToggleDoctorStatus;
public class ToggleDoctorStatusCommandValidator : AbstractValidator<ToggleDoctorStatusCommand>
{
    public ToggleDoctorStatusCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Doctor ID is required.");
    }
}
