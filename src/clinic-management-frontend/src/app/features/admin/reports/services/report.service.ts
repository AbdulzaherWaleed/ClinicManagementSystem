import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';

export interface PerformanceReportDto {
  totalBookings: number;
  completedBookings: number;
  cancelledOrNoShowBookings: number;
  completionRate: number;
  bookingsByVisitStage: { [key: string]: number };
  bookingsByDoctor?: { [key: string]: number };
}

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/reports`;

  getDoctorPerformance(doctorId: string, startDate: Date, endDate: Date): Observable<PerformanceReportDto> {
    const params = new HttpParams()
      .set('doctorId', doctorId)
      .set('startDate', this.formatDate(startDate))
      .set('endDate', this.formatDate(endDate, true));
    return this.http.get<PerformanceReportDto>(`${this.apiUrl}/doctor`, { params });
  }

  getStaffPerformance(employeeId: string, startDate: Date, endDate: Date): Observable<PerformanceReportDto> {
    const params = new HttpParams()
      .set('employeeId', employeeId)
      .set('startDate', this.formatDate(startDate))
      .set('endDate', this.formatDate(endDate, true));
    return this.http.get<PerformanceReportDto>(`${this.apiUrl}/staff`, { params });
  }

  private formatDate(date: Date, isEnd: boolean = false): string {
    const d = new Date(date);
    if (isEnd) {
      d.setHours(23, 59, 59, 999);
    } else {
      d.setHours(0, 0, 0, 0);
    }
    // Return yyyy-MM-ddTHH:mm:ss in local time
    const pad = (n: number) => n < 10 ? '0' + n : n;
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
  }
}
