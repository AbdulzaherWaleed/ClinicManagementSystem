using FluentValidation;

namespace ClinicManagement.Application.Patients.Commands.UploadPatientDocument;

public class UploadPatientDocumentCommandValidator : AbstractValidator<UploadPatientDocumentCommand>
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
    private static readonly string[] AllowedDocumentTypes = { "Prescription", "LabResult", "XRay", "Other" };

    public UploadPatientDocumentCommandValidator()
    {
        RuleFor(v => v.PatientId).NotEmpty().WithMessage("Patient ID is required.");
        
        RuleFor(v => v.DocumentType)
            .NotEmpty().WithMessage("Document type is required.")
            .Must(type => AllowedDocumentTypes.Contains(type))
            .WithMessage($"Document type must be one of: {string.Join(", ", AllowedDocumentTypes)}.");
            
        RuleFor(v => v.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .Must(name => AllowedExtensions.Any(ext => name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Only PDF and image files are allowed.");
            
        RuleFor(v => v.FileStream).NotNull().WithMessage("File content is required.");
        
        RuleFor(v => v.Notes).MaximumLength(500).When(v => v.Notes != null);
    }
}
