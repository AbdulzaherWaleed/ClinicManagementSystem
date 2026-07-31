using ClinicManagement.Application.Auth.DTOs;
using ClinicManagement.Domain.Enums;
using MediatR;

namespace ClinicManagement.Application.Auth.Commands.Register;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public UserRole Role { get; set; }
    public Guid? AssignedDoctorId { get; set; }
}