using FluentValidation;

namespace ClinicManagement.Application.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Appointment ID is required.");

        // Conditional: only validate dates when they are provided
        RuleFor(v => v.ScheduledStart)
            .NotEmpty().WithMessage("Scheduled Start time is required.")
            .When(v => v.ScheduledStart.HasValue);

        RuleFor(v => v.ScheduledEnd)
            .GreaterThan(v => v.ScheduledStart!.Value)
            .WithMessage("Scheduled End must be after Scheduled Start.")
            .When(v => v.ScheduledStart.HasValue && v.ScheduledEnd.HasValue);

        // If only End is provided without Start, it's ambiguous — reject
        RuleFor(v => v.ScheduledStart)
            .NotNull().WithMessage("ScheduledStart is required when ScheduledEnd is provided.")
            .When(v => v.ScheduledEnd.HasValue && !v.ScheduledStart.HasValue);
    }
}
