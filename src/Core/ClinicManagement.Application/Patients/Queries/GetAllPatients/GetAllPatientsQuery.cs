using ClinicManagement.Application.Patients.DTOs;
using MediatR;

using ClinicManagement.Application.Common.Models;

namespace ClinicManagement.Application.Patients.Queries.GetAllPatients;

public class GetAllPatientsQuery : IRequest<PaginatedList<PatientDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // v2.1 — Employee Scoping Support
    public List<Guid>? RestrictToDoctorIds { get; set; }
}