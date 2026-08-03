using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Reports.DTOs;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Reports.Queries.GetDoctorPerformance;

public class GetDoctorPerformanceQueryHandler : IRequestHandler<GetDoctorPerformanceQuery, PerformanceReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetDoctorPerformanceQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PerformanceReportDto> Handle(GetDoctorPerformanceQuery request, CancellationToken cancellationToken)
    {
        var appointmentData = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == request.DoctorId && 
                        a.ScheduledStart >= request.StartDate && 
                        a.ScheduledStart <= request.EndDate &&
                        !a.IsDeleted)
            .Select(a => new {
                a.Status,
                a.VisitStage
            })
            .ToListAsync(cancellationToken);

        var total = appointmentData.Count;
        var completed = appointmentData.Count(a => a.Status == AppointmentStatus.Completed);
        var cancelledOrNoShow = appointmentData.Count(a => a.Status == AppointmentStatus.Cancelled || a.Status == AppointmentStatus.NoShow);
        
        var completionRate = total > 0 ? Math.Round((double)completed / total * 100, 2) : 0;

        var byStage = appointmentData
            .GroupBy(a => a.VisitStage.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new PerformanceReportDto
        {
            TotalBookings = total,
            CompletedBookings = completed,
            CancelledOrNoShowBookings = cancelledOrNoShow,
            CompletionRate = completionRate,
            BookingsByVisitStage = byStage
        };
    }
}
