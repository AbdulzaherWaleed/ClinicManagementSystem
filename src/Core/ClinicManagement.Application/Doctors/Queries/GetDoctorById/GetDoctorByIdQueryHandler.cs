using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Doctors.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Doctors.Queries.GetDoctorById;

public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, DoctorDto?>
{
    private readonly IApplicationDbContext _context;

    public GetDoctorByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DoctorDto?> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Doctors
            .Where(d => d.Id == request.Id)
            .Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Title = d.Title,
                Bio = d.Bio,
                PhoneNumber = d.PhoneNumber,
                Email = d.Email,
                PrimarySpecialtyName = d.PrimarySpecialty != null ? d.PrimarySpecialty.Name : null,
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}