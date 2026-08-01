using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Doctors.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

using ClinicManagement.Application.Common.Models;

namespace ClinicManagement.Application.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, PaginatedList<DoctorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllDoctorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<DoctorDto>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Doctors
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
            });

        return await PaginatedList<DoctorDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
    }
}