using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Doctors.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

using ClinicManagement.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ClinicManagement.Application.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, PaginatedList<DoctorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ICurrentUserService _currentUserService;

    public GetAllDoctorsQueryHandler(IApplicationDbContext context, IMemoryCache cache, ICurrentUserService currentUserService)
    {
        _context = context;
        _cache = cache;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<DoctorDto>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
    {
        var queryable = _context.Doctors.AsNoTracking().Where(d => !d.IsDeleted).AsQueryable();

        // v2.1 — Employee Scoping Support
        string restrictKey = "all";
        if (_currentUserService.Role == "Employee")
        {
            var allowedIds = _currentUserService.AssignedDoctorIds;
            queryable = queryable.Where(d => allowedIds.Contains(d.Id));
            restrictKey = string.Join(",", allowedIds.OrderBy(id => id));
        }

        var cacheKey = $"GetAllDoctors_{request.PageNumber}_{request.PageSize}_{restrictKey}";

        if (_cache.TryGetValue(cacheKey, out PaginatedList<DoctorDto>? cachedResult) && cachedResult != null)
        {
            return cachedResult;
        }

        var query = queryable
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

        var result = await PaginatedList<DoctorDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
            .AddExpirationToken(DoctorCacheTokens.GetChangeToken());

        _cache.Set(cacheKey, result, cacheEntryOptions);
        
        return result;
    }
}