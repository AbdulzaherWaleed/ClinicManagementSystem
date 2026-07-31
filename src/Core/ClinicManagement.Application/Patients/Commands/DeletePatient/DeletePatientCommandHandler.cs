using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Commands.DeletePatient;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
            throw new NotFoundException(nameof(Domain.Entities.Patients.Patient), request.Id);

        // Soft delete — handled automatically by AuditableEntitySaveChangesInterceptor
        // when the entry is marked as EntityState.Deleted
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync(cancellationToken);
    }
}