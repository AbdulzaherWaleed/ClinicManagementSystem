using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Doctors;
using MediatR;

namespace ClinicManagement.Application.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateDoctorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = new Doctor
        {
            FullName = request.FullName,
            Title = request.Title,
            Bio = request.Bio,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            PrimarySpecialtyId = request.PrimarySpecialtyId,
            IsActive = true
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(cancellationToken);

        return doctor.Id;
    }
}