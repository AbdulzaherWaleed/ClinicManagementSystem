using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Common.Models;
using ClinicManagement.Application.Patients.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Queries.GetPatientDocuments;

public class GetPatientDocumentsQueryHandler : IRequestHandler<GetPatientDocumentsQuery, PaginatedList<PatientDocumentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPatientDocumentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<PatientDocumentDto>> Handle(GetPatientDocumentsQuery request, CancellationToken cancellationToken)
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

        // 2. Query with Select projection directly to DTO
        var query = _context.PatientDocuments
            .AsNoTracking()
            .Where(d => d.PatientId == request.PatientId && !d.IsDeleted)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new PatientDocumentDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                OriginalFileName = d.OriginalFileName,
                FileType = d.FileType,
                Notes = d.Notes,
                CreatedAt = d.CreatedAt
            });

        // 3. Paginate
        return await PaginatedList<PatientDocumentDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
    }
}
