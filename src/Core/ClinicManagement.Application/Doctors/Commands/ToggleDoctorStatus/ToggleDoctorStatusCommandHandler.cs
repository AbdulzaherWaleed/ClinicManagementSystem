using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;

namespace ClinicManagement.Application.Doctors.Commands.ToggleDoctorStatus;

public class ToggleDoctorStatusCommandHandler : IRequestHandler<ToggleDoctorStatusCommand>
{
    private readonly IApplicationDbContext _context;

    public ToggleDoctorStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ToggleDoctorStatusCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors.FindAsync(new object[] { request.Id }, cancellationToken);

        if (doctor == null)
            throw new NotFoundException(nameof(doctor), request.Id);

        doctor.IsActive = !doctor.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
