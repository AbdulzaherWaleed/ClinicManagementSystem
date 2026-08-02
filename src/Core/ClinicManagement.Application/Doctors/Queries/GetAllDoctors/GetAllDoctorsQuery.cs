using ClinicManagement.Application.Doctors.DTOs;
using MediatR;
using ClinicManagement.Application.Common.Models;

namespace ClinicManagement.Application.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQuery : IRequest<PaginatedList<DoctorDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // v2.1 — Employee Scoping Support
    public List<Guid>? RestrictToDoctorIds { get; set; }
}