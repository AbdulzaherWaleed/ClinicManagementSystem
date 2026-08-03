using FluentValidation;

namespace ClinicManagement.Application.Appointments.Commands.ChangeAppointmentStatus;

public class ChangeAppointmentStatusCommandValidator : AbstractValidator<ChangeAppointmentStatusCommand>
{
    public ChangeAppointmentStatusCommandValidator()
    {
        RuleFor(v => v.AppointmentId)
            .NotEmpty().WithMessage("AppointmentId is required.");

        RuleFor(v => v.NewStatus)
            .IsInEnum().WithMessage("NewStatus must be a valid AppointmentStatus.");

        RuleFor(v => v.ChangedByEmployeeId)
            .NotEmpty().WithMessage("ChangedByEmployeeId is required.");
    }
}
