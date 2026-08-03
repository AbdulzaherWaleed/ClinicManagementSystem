using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Patients.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

using ClinicManagement.Application.Common.Models;

namespace ClinicManagement.Application.Patients.Queries.GetAllPatients;

public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, PaginatedList<PatientDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPatientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<PatientDto>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var queryable = _context.Patients.AsNoTracking().Where(p => !p.IsDeleted).AsQueryable();

        // v2.1 — Employee Scoping Support
        if (request.RestrictToDoctorIds is { Count: > 0 })
        {
            // A patient is visible if they have AT LEAST ONE appointment with any of the restricted doctors
            queryable = queryable.Where(p => p.Appointments.Any(a => !a.IsDeleted && request.RestrictToDoctorIds.Contains(a.DoctorId)));
        }

        var patientsQuery = queryable
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                PhoneNumber = p.PhoneNumber,
                Email = p.Email,
                Address = p.Address,
                NationalId = p.NationalId,
                MedicalNotes = p.MedicalNotes,
                CreatedAt = p.CreatedAt
            });
            
        return await PaginatedList<PatientDto>.CreateAsync(patientsQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}