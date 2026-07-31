using ClinicManagement.Application.Common.Interfaces;

namespace ClinicManagement.Infrastructure.Common;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}