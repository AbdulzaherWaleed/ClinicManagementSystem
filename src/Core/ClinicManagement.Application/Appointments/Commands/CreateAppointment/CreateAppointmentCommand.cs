using ClinicManagement.Domain.Enums;
using MediatR;

namespace ClinicManagement.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommand : IRequest<Guid>
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    
    public VisitStage VisitStage { get; set; } = VisitStage.Checkup;
    public string? VisitType { get; set; }
    
    public string? Reason { get; set; }
    
    // Extracted from JWT context in the Controller
    public Guid? CreatedByEmployeeId { get; set; }
}
