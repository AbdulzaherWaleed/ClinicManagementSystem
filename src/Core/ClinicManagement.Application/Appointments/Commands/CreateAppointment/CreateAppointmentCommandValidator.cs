using FluentValidation;

namespace ClinicManagement.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        
        RuleFor(x => x.ScheduledStart)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Scheduled start time cannot be in the past.");
            
        RuleFor(x => x.ScheduledEnd)
            .NotEmpty()
            .GreaterThan(x => x.ScheduledStart)
            .WithMessage("Scheduled end time must be after the start time.");
            
        RuleFor(x => x.VisitStage).IsInEnum();
        
        RuleFor(x => x.VisitType).NotEmpty().WithMessage("نوع الكشف مطلوب").MaximumLength(100);
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
