using ClinicManagement.Application.Patients.DTOs;
using MediatR;

namespace ClinicManagement.Application.Patients.Queries.GetPatientById;

public class GetPatientByIdQuery : IRequest<PatientDto?>
{
    public Guid Id { get; set; }
}