using FluentValidation;

namespace ClinicManagement.Application.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Appointment ID is required.");

        RuleFor(v => v.ScheduledStart)
            .NotEmpty().WithMessage("Scheduled Start time is required.");
            
        RuleFor(v => v.ScheduledEnd)
            .GreaterThan(v => v.ScheduledStart).WithMessage("Scheduled End must be after Scheduled Start.");
    }
}
