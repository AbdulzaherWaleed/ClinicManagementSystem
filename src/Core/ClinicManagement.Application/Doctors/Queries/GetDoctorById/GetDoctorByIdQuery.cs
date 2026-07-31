using ClinicManagement.Application.Doctors.DTOs;
using MediatR;

namespace ClinicManagement.Application.Doctors.Queries.GetDoctorById;

public class GetDoctorByIdQuery : IRequest<DoctorDto?>
{
    public Guid Id { get; set; }
}