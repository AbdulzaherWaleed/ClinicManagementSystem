using ClinicManagement.Application.Common.Models;
using ClinicManagement.Application.Patients.DTOs;
using MediatR;

namespace ClinicManagement.Application.Patients.Queries.GetPatientDocuments;

public class GetPatientDocumentsQuery : IRequest<PaginatedList<PatientDocumentDto>>
{
    public Guid PatientId { get; set; }
    public List<Guid>? RestrictToDoctorIds { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
