using ClinicManagement.Application.Appointments.DTOs;
using MediatR;
using ClinicManagement.Application.Common.Models;

namespace ClinicManagement.Application.Appointments.Queries.GetAppointments;

/// <summary>
/// Advanced Search Query — all filters are optional and combinable (AND logic).
/// Matches Brief §4.1 Advanced Search Bookings exactly.
/// </summary>
public class GetAppointmentsQuery : IRequest<PaginatedList<AppointmentDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? PatientName { get; set; }
    public string? Phone { get; set; }
    public Guid? DoctorId { get; set; }
    public string? VisitType { get; set; }       // Botox, ENT, Ophthalmology, etc.
    public string? VisitStage { get; set; }       // Checkup | Consultation | Procedure
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Status { get; set; }

    // For Employee scoping — server-side enforced
    public List<Guid>? RestrictToDoctorIds { get; set; }
}
