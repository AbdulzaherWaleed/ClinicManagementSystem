using FluentValidation;
namespace ClinicManagement.Application.Patients.Commands.DeletePatient;
public class DeletePatientCommandValidator : AbstractValidator<DeletePatientCommand>
{
    public DeletePatientCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Patient ID is required.");
    }
}
