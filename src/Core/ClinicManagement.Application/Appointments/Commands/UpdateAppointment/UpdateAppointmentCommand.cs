using ClinicManagement.Domain.Enums;
using MediatR;

namespace ClinicManagement.Application.Appointments.Commands.UpdateAppointment;

/// <summary>
/// PATCH semantics: only non-null fields are applied. Null = "don't touch".
/// </summary>
public class UpdateAppointmentCommand : IRequest
{
    public Guid Id { get; set; }

    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }

    public AppointmentStatus? Status { get; set; }
    public VisitStage? VisitStage { get; set; }
    public string? VisitType { get; set; }
    public string? Reason { get; set; }
}
