using FluentValidation;
namespace ClinicManagement.Application.Patients.Commands.DeletePatientDocument;
public class DeletePatientDocumentCommandValidator : AbstractValidator<DeletePatientDocumentCommand>
{
    public DeletePatientDocumentCommandValidator()
    {
        RuleFor(v => v.PatientId).NotEmpty().WithMessage("Patient ID is required.");
        RuleFor(v => v.DocumentId).NotEmpty().WithMessage("Document ID is required.");
    }
}
