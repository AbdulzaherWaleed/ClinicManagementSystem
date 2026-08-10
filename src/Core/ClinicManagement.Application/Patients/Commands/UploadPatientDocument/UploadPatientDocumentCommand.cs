using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Patients;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Commands.UploadPatientDocument;

public class UploadPatientDocumentCommand : IRequest<Guid>
{
    public Guid PatientId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    
    // File details
    public string FileName { get; set; } = string.Empty;
    public Stream FileStream { get; set; } = default!;
    
    // IDOR
    public List<Guid>? RestrictToDoctorIds { get; set; }
}

public class UploadPatientDocumentCommandHandler : IRequestHandler<UploadPatientDocumentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public UploadPatientDocumentCommandHandler(IApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<Guid> Handle(UploadPatientDocumentCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate file using magic bytes (PDF or Images only)
        var fileType = ClinicManagement.Application.Common.Helpers.FileValidationHelper.GetFileType(request.FileStream);
        
        if (fileType == "Unknown")
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("File", "الملف يجب أن يكون بصيغة PDF أو صورة صالحة.") });

        // 2. IDOR Check
        if (request.RestrictToDoctorIds is { Count: > 0 })
        {
            var hasAccess = await _context.Appointments
                .AnyAsync(a => a.PatientId == request.PatientId
                            && request.RestrictToDoctorIds.Contains(a.DoctorId),
                          cancellationToken);

            if (!hasAccess)
                throw new ForbiddenAccessException();
        }

        var patientExists = await _context.Patients.AnyAsync(p => p.Id == request.PatientId, cancellationToken);
        if (!patientExists)
            throw new NotFoundException(nameof(Patient), request.PatientId);

        // 3. Save file using storage service (creates path in configured directory e.g., App_Data/PatientDocuments)
        request.FileStream.Position = 0; // reset after magic bytes check
        var savedFilePath = await _fileStorageService.SaveFileAsync(request.FileStream, request.FileName, "PatientDocuments", cancellationToken);

        // 4. Create entity
        var document = new PatientDocument
        {
            PatientId = request.PatientId,
            DocumentType = request.DocumentType,
            OriginalFileName = request.FileName,
            FilePath = savedFilePath,
            FileType = fileType,
            Notes = request.Notes
        };

        _context.PatientDocuments.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        return document.Id;
    }
}
