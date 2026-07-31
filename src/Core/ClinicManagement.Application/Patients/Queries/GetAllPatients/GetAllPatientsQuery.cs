using ClinicManagement.Application.Patients.DTOs;
using MediatR;

namespace ClinicManagement.Application.Patients.Queries.GetAllPatients;

public class GetAllPatientsQuery : IRequest<List<PatientDto>>
{
}