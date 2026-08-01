using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Appointments;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify Patient exists
        var patientExists = await _context.Patients.AnyAsync(p => p.Id == request.PatientId && !p.IsDeleted, cancellationToken);
        if (!patientExists)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("PatientId", "Patient does not exist.")
            });

        // 2. Verify Doctor exists
        var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId && !d.IsDeleted, cancellationToken);
        if (!doctorExists)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("DoctorId", "Doctor does not exist.")
            });

        // 3. Check for conflicting appointments for this doctor
        var hasConflict = await _context.Appointments
            .AnyAsync(a => a.DoctorId == request.DoctorId && 
                           !a.IsDeleted && 
                           a.Status != AppointmentStatus.Cancelled &&
                           request.ScheduledStart < a.ScheduledEnd && 
                           request.ScheduledEnd > a.ScheduledStart, 
                      cancellationToken);

        if (hasConflict)
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new("ScheduledStart", "Doctor already has an appointment during this time slot.")
            });

        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledEnd,
            Status = AppointmentStatus.Scheduled,
            VisitStage = request.VisitStage,
            VisitType = request.VisitType,
            Reason = request.Reason,
            CreatedByEmployeeId = request.CreatedByEmployeeId
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }
}
