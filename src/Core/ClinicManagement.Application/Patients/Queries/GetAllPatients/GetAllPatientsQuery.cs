using ClinicManagement.Application.Patients.DTOs;
using MediatR;

namespace ClinicManagement.Application.Patients.Queries.GetAllPatients;

public class GetAllPatientsQuery : IRequest<List<PatientDto>>
{
    // v2.1 — Employee Scoping Support
    public List<Guid>? RestrictToDoctorIds { get; set; }
}