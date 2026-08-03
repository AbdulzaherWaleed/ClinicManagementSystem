using MediatR;

namespace ClinicManagement.Application.Appointments.Queries.ExportAppointments;

public class ExportAppointmentsQuery : IRequest<byte[]>
{
    public string? PatientName { get; set; }
    public string? Phone { get; set; }
    public Guid? DoctorId { get; set; }
    public string? VisitType { get; set; }
    public string? VisitStage { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Status { get; set; }
    public List<Guid>? RestrictToDoctorIds { get; set; }
}
