using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Entities.Doctors;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using ClinicManagement.Application.Doctors.Queries;

namespace ClinicManagement.Application.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public CreateDoctorCommandHandler(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
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

        DoctorCacheTokens.Clear();

        return doctor.Id;
    }
}