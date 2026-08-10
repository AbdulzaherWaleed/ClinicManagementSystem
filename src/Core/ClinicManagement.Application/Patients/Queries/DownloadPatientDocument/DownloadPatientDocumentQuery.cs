using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Queries.DownloadPatientDocument;

public class DownloadPatientDocumentQuery : IRequest<(Stream FileStream, string FileName, string ContentType)>
{
    public Guid PatientId { get; set; }
    public Guid DocumentId { get; set; }

    // IDOR
    public List<Guid>? RestrictToDoctorIds { get; set; }
}

public class DownloadPatientDocumentQueryHandler : IRequestHandler<DownloadPatientDocumentQuery, (Stream FileStream, string FileName, string ContentType)>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public DownloadPatientDocumentQueryHandler(IApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<(Stream FileStream, string FileName, string ContentType)> Handle(DownloadPatientDocumentQuery request, CancellationToken cancellationToken)
    {
        // 1. IDOR Check
        if (request.RestrictToDoctorIds is { Count: > 0 })
        {
            var hasAccess = await _context.Appointments
                .AnyAsync(a => a.PatientId == request.PatientId
                            && request.RestrictToDoctorIds.Contains(a.DoctorId),
                          cancellationToken);

            if (!hasAccess)
                throw new ForbiddenAccessException();
        }

        var document = await _context.PatientDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId && d.PatientId == request.PatientId, cancellationToken);

        if (document is null)
            throw new NotFoundException(nameof(Domain.Entities.Patients.PatientDocument), request.DocumentId);

        var stream = await _fileStorageService.GetFileAsync(document.FilePath, cancellationToken);
        var contentType = document.FileType == "PDF" ? "application/pdf" : "application/octet-stream";
        
        return (stream, document.OriginalFileName, contentType);
    }
}
