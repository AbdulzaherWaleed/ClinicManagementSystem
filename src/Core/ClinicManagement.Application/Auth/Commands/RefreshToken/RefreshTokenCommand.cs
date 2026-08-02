using ClinicManagement.Application.Auth.DTOs;
using MediatR;

namespace ClinicManagement.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
