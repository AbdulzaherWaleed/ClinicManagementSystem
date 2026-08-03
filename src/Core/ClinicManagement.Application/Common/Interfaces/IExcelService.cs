using ClinicManagement.Application.Appointments.DTOs;

namespace ClinicManagement.Application.Common.Interfaces;

public interface IExcelService
{
    byte[] ExportAppointmentsToExcel(IEnumerable<AppointmentDto> appointments);
}
