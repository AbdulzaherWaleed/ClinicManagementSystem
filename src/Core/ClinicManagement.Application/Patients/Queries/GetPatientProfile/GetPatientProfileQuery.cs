using ClinicManagement.Application.Patients.DTOs;
using MediatR;

namespace ClinicManagement.Application.Patients.Queries.GetPatientProfile;

public class GetPatientProfileQuery : IRequest<PatientProfileDto?>
{
    public Guid Id { get; set; }

    /// <summary>
    /// If set (Employee role), the patient must have at least one appointment
    /// with one of these doctors, otherwise ForbiddenAccessException is thrown.
    /// </summary>
    public List<Guid>? RestrictToDoctorIds { get; set; }
}
