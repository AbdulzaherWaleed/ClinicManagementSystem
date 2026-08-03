using ClinicManagement.Application.Common.Interfaces;
using ClinicManagement.Application.Reports.DTOs;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Application.Reports.Queries.GetStaffPerformance;

public class GetStaffPerformanceQueryHandler : IRequestHandler<GetStaffPerformanceQuery, PerformanceReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetStaffPerformanceQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PerformanceReportDto> Handle(GetStaffPerformanceQuery request, CancellationToken cancellationToken)
    {
        var assignedDoctorIds = await _context.EmployeeDoctorAssignments
            .Where(e => e.EmployeeId == request.EmployeeId && !e.IsDeleted)
            .Select(e => e.DoctorId)
            .ToListAsync(cancellationToken);

        var appointmentData = await _context.Appointments
            .AsNoTracking()
            .Where(a => (a.CreatedByEmployeeId == request.EmployeeId || assignedDoctorIds.Contains(a.DoctorId)) && 
                        a.ScheduledStart >= request.StartDate && 
                        a.ScheduledStart <= request.EndDate &&
                        !a.IsDeleted)
            .Select(a => new {
                a.Status,
                a.VisitStage,
                DoctorName = a.Doctor != null ? a.Doctor.FullName : null
            })
            .ToListAsync(cancellationToken);

        var total = appointmentData.Count;
        var completed = appointmentData.Count(a => a.Status == AppointmentStatus.Completed);
        var cancelledOrNoShow = appointmentData.Count(a => a.Status == AppointmentStatus.Cancelled || a.Status == AppointmentStatus.NoShow);
        
        var completionRate = total > 0 ? Math.Round((double)completed / total * 100, 2) : 0;

        var byStage = appointmentData
            .GroupBy(a => a.VisitStage.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var byDoctor = appointmentData
            .Where(a => a.DoctorName != null)
            .GroupBy(a => a.DoctorName!)
            .ToDictionary(g => g.Key, g => g.Count());

        return new PerformanceReportDto
        {
            TotalBookings = total,
            CompletedBookings = completed,
            CancelledOrNoShowBookings = cancelledOrNoShow,
            CompletionRate = completionRate,
            BookingsByVisitStage = byStage,
            BookingsByDoctor = byDoctor
        };
    }
}
