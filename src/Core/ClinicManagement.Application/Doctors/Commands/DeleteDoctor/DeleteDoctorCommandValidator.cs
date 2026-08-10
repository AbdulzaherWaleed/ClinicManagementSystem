using FluentValidation;
namespace ClinicManagement.Application.Doctors.Commands.DeleteDoctor;
public class DeleteDoctorCommandValidator : AbstractValidator<DeleteDoctorCommand>
{
    public DeleteDoctorCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Doctor ID is required.");
    }
}
