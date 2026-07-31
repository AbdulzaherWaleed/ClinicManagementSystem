using ClinicManagement.Application.Doctors.DTOs;
using MediatR;

namespace ClinicManagement.Application.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQuery : IRequest<List<DoctorDto>>
{
}