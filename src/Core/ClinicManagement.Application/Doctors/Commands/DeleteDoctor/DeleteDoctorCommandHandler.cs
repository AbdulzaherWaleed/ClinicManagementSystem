using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using ClinicManagement.Application.Doctors.Queries;

namespace ClinicManagement.Application.Doctors.Commands.DeleteDoctor;

public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public DeleteDoctorCommandHandler(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors.FindAsync(new object[] { request.Id }, cancellationToken);

        if (doctor == null)
            throw new NotFoundException(nameof(doctor), request.Id);

        doctor.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        DoctorCacheTokens.Clear();
    }
}
