using ClinicManagement.Application.Appointments.DTOs;
using ClinicManagement.Application.Common.Interfaces;
using ClosedXML.Excel;

namespace ClinicManagement.Infrastructure.Services;

public class ExcelService : IExcelService
{
    public byte[] ExportAppointmentsToExcel(IEnumerable<AppointmentDto> appointments)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Appointments");

        // Add Headers
        var currentRow = 1;
        worksheet.Cell(currentRow, 1).Value = "تاريخ الحجز";
        worksheet.Cell(currentRow, 2).Value = "المريض";
        worksheet.Cell(currentRow, 3).Value = "الهاتف";
        worksheet.Cell(currentRow, 4).Value = "الطبيب";
        worksheet.Cell(currentRow, 5).Value = "مرحلة الزيارة";
        worksheet.Cell(currentRow, 6).Value = "نوع الكشف";
        worksheet.Cell(currentRow, 7).Value = "الحالة";

        // Style Headers
        var headerRange = worksheet.Range(1, 1, 1, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        foreach (var appointment in appointments)
        {
            currentRow++;
            // Format dates
            worksheet.Cell(currentRow, 1).Value = appointment.ScheduledStart.ToString("yyyy-MM-dd HH:mm");
            worksheet.Cell(currentRow, 2).Value = appointment.PatientName;
            
            // Format phone as text by prepending single quote
            worksheet.Cell(currentRow, 3).Value = $"'{appointment.PatientPhone}";

            worksheet.Cell(currentRow, 4).Value = appointment.DoctorName;
            worksheet.Cell(currentRow, 5).Value = appointment.VisitStage.ToString();
            worksheet.Cell(currentRow, 6).Value = appointment.VisitType;
            worksheet.Cell(currentRow, 7).Value = appointment.Status.ToString();
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
