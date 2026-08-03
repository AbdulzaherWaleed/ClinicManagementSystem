import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { CreatedEntityResponse } from '../../patients/models/patient.models';
import { environment } from '../../../../environments/environment';
import { 
  AppointmentDto, 
  CancelAppointmentCommand, 
  CreateAppointmentCommand, 
  GetAppointmentsQuery, 
  UpdateAppointmentCommand 
} from '../models/appointment.models';

import { PaginatedList } from '../../../core/models/pagination.models';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/appointments`;

  getAppointments(query: GetAppointmentsQuery & { pageNumber?: number, pageSize?: number }): Observable<PaginatedList<AppointmentDto>> {
    let params = new HttpParams();
    
    if (query.patientName) params = params.set('patientName', query.patientName);
    if (query.phone) params = params.set('phone', query.phone);
    if (query.doctorId) params = params.set('doctorId', query.doctorId);
    if (query.visitType) params = params.set('visitType', query.visitType);
    if (query.visitStage) params = params.set('visitStage', query.visitStage);
    if (query.dateFrom) params = params.set('dateFrom', query.dateFrom);
    if (query.dateTo) params = params.set('dateTo', query.dateTo);
    if (query.status) params = params.set('status', query.status);
    
    params = params.set('pageNumber', query.pageNumber || 1);
    params = params.set('pageSize', query.pageSize || 100);

    return this.http.get<PaginatedList<AppointmentDto>>(this.apiUrl, { params });
  }

  getAppointmentById(id: string): Observable<AppointmentDto> {
    return this.http.get<AppointmentDto>(`${this.apiUrl}/${id}`);
  }

  createAppointment(command: CreateAppointmentCommand): Observable<string> {
    return this.http
      .post<CreatedEntityResponse>(this.apiUrl, command)
      .pipe(map((response) => response.id));
  }

  updateAppointment(id: string, command: UpdateAppointmentCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  cancelAppointment(id: string, command: CancelAppointmentCommand): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/cancel`, command);
  }

  changeAppointmentStatus(id: string, newStatus: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/status`, `"${newStatus}"`, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  exportAppointmentsToExcel(query?: GetAppointmentsQuery): Observable<Blob> {
    let params = new HttpParams();
    
    if (query) {
      if (query.patientName) params = params.set('patientName', query.patientName);
      if (query.phone) params = params.set('phone', query.phone);
      if (query.doctorId) params = params.set('doctorId', query.doctorId);
      if (query.visitType) params = params.set('visitType', query.visitType);
      if (query.visitStage) params = params.set('visitStage', query.visitStage);
      if (query.dateFrom) params = params.set('dateFrom', query.dateFrom);
      if (query.dateTo) params = params.set('dateTo', query.dateTo);
      if (query.status) params = params.set('status', query.status);
    }
    
    return this.http.get(`${this.apiUrl}/export`, { params, responseType: 'blob' });
  }
}
