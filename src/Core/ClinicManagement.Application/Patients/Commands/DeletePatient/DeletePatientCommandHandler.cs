using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Commands.DeletePatient;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeletePatientCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
            throw new NotFoundException(nameof(Domain.Entities.Patients.Patient), request.Id);

        // IDOR check: Employee may only delete patients within their assigned doctor scope
        if (_currentUserService.Role == "Employee")
        {
            var hasLinkedAppointment = await _context.Appointments
                .AnyAsync(a => a.PatientId == patient.Id
                            && _currentUserService.AssignedDoctorIds.Contains(a.DoctorId),
                          cancellationToken);

            if (!hasLinkedAppointment)
                throw new ForbiddenAccessException();
        }
        else if (_currentUserService.Role == "Doctor")
        {
            var hasLinkedAppointment = await _context.Appointments
                .AnyAsync(a => a.PatientId == patient.Id
                            && a.DoctorId == _currentUserService.UserId,
                          cancellationToken);

            if (!hasLinkedAppointment)
                throw new ForbiddenAccessException();
        }

        // Soft delete — handled automatically by AuditableEntitySaveChangesInterceptor
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync(cancellationToken);
    }
}