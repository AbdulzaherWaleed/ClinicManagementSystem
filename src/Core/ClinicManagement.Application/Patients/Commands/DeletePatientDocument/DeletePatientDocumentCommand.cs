using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Commands.DeletePatientDocument;

public class DeletePatientDocumentCommand : IRequest
{
    public Guid PatientId { get; set; }
    public Guid DocumentId { get; set; }
    
    // IDOR
    public List<Guid>? RestrictToDoctorIds { get; set; }
}

public class DeletePatientDocumentCommandHandler : IRequestHandler<DeletePatientDocumentCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePatientDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeletePatientDocumentCommand request, CancellationToken cancellationToken)
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
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId && d.PatientId == request.PatientId, cancellationToken);

        if (document is null)
            throw new NotFoundException(nameof(Domain.Entities.Patients.PatientDocument), request.DocumentId);

        // Soft delete
        _context.PatientDocuments.Remove(document);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
