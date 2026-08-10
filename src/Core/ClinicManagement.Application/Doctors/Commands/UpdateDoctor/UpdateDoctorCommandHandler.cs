using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDoctorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (doctor == null)
            throw new NotFoundException(nameof(Domain.Entities.Doctors.Doctor), request.Id);

        // Apply only the explicitly provided fields (PATCH semantics)
        if (request.FullName != null)          doctor.FullName          = request.FullName;
        if (request.Title != null)             doctor.Title             = request.Title;
        if (request.Bio != null)               doctor.Bio               = request.Bio;
        if (request.PhoneNumber != null)       doctor.PhoneNumber       = request.PhoneNumber;
        if (request.Email != null)             doctor.Email             = request.Email;
        if (request.PrimarySpecialtyId.HasValue) doctor.PrimarySpecialtyId = request.PrimarySpecialtyId;
        if (request.IsActive.HasValue)         doctor.IsActive          = request.IsActive.Value;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
