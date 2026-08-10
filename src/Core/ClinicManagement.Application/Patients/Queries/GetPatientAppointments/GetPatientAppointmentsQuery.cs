using ClinicManagement.Application.Appointments.DTOs;
using ClinicManagement.Application.Common.Models;
using MediatR;

namespace ClinicManagement.Application.Patients.Queries.GetPatientAppointments;

public class GetPatientAppointmentsQuery : IRequest<PaginatedList<AppointmentDto>>
{
    public Guid PatientId { get; set; }
    public List<Guid>? RestrictToDoctorIds { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
