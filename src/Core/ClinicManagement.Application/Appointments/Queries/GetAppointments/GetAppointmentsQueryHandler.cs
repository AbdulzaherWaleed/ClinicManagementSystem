using ClinicManagement.Application.Appointments.DTOs;
using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

using ClinicManagement.Application.Common.Models;

namespace ClinicManagement.Application.Appointments.Queries.GetAppointments;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, PaginatedList<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAppointmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.PrimarySpecialty)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        // Apply all optional filters (AND logic)
        if (!string.IsNullOrWhiteSpace(request.PatientName))
            query = query.Where(a => a.Patient.FullName.Contains(request.PatientName));

        if (!string.IsNullOrWhiteSpace(request.Phone))
            query = query.Where(a => a.Patient.PhoneNumber != null &&
                                     a.Patient.PhoneNumber.Contains(request.Phone));

        if (request.DoctorId.HasValue)
            query = query.Where(a => a.DoctorId == request.DoctorId.Value);

        if (!string.IsNullOrWhiteSpace(request.VisitType))
            query = query.Where(a => a.VisitType != null && a.VisitType.Contains(request.VisitType));

        if (!string.IsNullOrWhiteSpace(request.VisitStage) &&
            Enum.TryParse<VisitStage>(request.VisitStage, out var visitStage))
            query = query.Where(a => a.VisitStage == visitStage);

        if (request.DateFrom.HasValue)
            query = query.Where(a => a.ScheduledStart >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(a => a.ScheduledStart <= request.DateTo.Value.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<AppointmentStatus>(request.Status, out var status))
            query = query.Where(a => a.Status == status);

        // Employee scoping — security boundary enforced server-side (Brief §2.1)
        if (request.RestrictToDoctorIds is { Count: > 0 })
            query = query.Where(a => request.RestrictToDoctorIds.Contains(a.DoctorId));

        var mappedQuery = query
            .OrderByDescending(a => a.ScheduledStart)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                PatientPhone = a.Patient.PhoneNumber,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                DoctorSpecialty = a.Doctor.PrimarySpecialty != null ? a.Doctor.PrimarySpecialty.Name : null,
                CreatedByEmployeeId = a.CreatedByEmployeeId,
                ScheduledStart = a.ScheduledStart,
                ScheduledEnd = a.ScheduledEnd,
                Status = a.Status.ToString(),
                Reason = a.Reason,
                CancellationReason = a.CancellationReason,
                VisitStage = a.VisitStage.ToString(),
                VisitType = a.VisitType,
                CreatedAt = a.CreatedAt
            });

        return await PaginatedList<AppointmentDto>.CreateAsync(mappedQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}
