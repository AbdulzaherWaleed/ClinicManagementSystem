using ClinicManagement.Application.Auth.DTOs;
using MediatR;

namespace ClinicManagement.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}