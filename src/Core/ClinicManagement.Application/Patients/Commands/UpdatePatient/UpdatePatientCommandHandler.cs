using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePatientCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient is null)
            throw new NotFoundException(nameof(Domain.Entities.Patients.Patient), request.Id);

        // IDOR check: Employee may only update patients who have an appointment
        // with one of their assigned doctors
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

        // Apply only explicitly provided fields (PATCH semantics)
        if (request.FullName != null)              patient.FullName              = request.FullName;
        if (request.DateOfBirth.HasValue)          patient.DateOfBirth           = request.DateOfBirth;
        if (request.Gender != null)                patient.Gender                = request.Gender;
        if (request.PhoneNumber != null)           patient.PhoneNumber           = request.PhoneNumber;
        if (request.Email != null)                 patient.Email                 = request.Email;
        if (request.Address != null)               patient.Address               = request.Address;
        if (request.NationalId != null)            patient.NationalId            = request.NationalId;
        if (request.MedicalNotes != null)          patient.MedicalNotes          = request.MedicalNotes;
        if (request.EmergencyContactName != null)  patient.EmergencyContactName  = request.EmergencyContactName;
        if (request.EmergencyContactPhone != null) patient.EmergencyContactPhone = request.EmergencyContactPhone;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
