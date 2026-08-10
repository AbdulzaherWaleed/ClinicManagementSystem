using FluentValidation;

namespace ClinicManagement.Application.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Appointment ID is required.");

        RuleFor(v => v.CancellationReason)
            .MaximumLength(500).WithMessage("Cancellation reason must not exceed 500 characters.");
    }
}
