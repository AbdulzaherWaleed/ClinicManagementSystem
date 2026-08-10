import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { CreatedEntityResponse } from '../../../patients/models/patient.models';
import { DoctorDto, CreateDoctorCommand } from '../models/doctor.models';
import { environment } from '../../../../../environments/environment';
import { PaginatedList } from '../../../../core/models/pagination.models';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/doctors`;

  getDoctors(pageNumber: number = 1, pageSize: number = 100): Observable<PaginatedList<DoctorDto>> {
    return this.http.get<PaginatedList<DoctorDto>>(`${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  createDoctor(command: CreateDoctorCommand): Observable<string> {
    return this.http
      .post<CreatedEntityResponse>(this.apiUrl, command)
      .pipe(map((response) => response.id));
  }

  updateDoctor(id: string, command: Partial<CreateDoctorCommand> & { isActive?: boolean }): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, command);
  }

  toggleStatus(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/toggle-status`, {});
  }

  deleteDoctor(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getLicenses(doctorId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${doctorId}/licenses`);
  }

  uploadLicense(doctorId: string, file: File, licenseNumber: string, expiryDate: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    if (licenseNumber) formData.append('licenseNumber', licenseNumber);
    if (expiryDate) formData.append('expiryDate', expiryDate);
    
    return this.http.post(`${this.apiUrl}/${doctorId}/licenses`, formData);
  }

  downloadLicense(doctorId: string, licenseId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${doctorId}/licenses/${licenseId}/download`, {
      responseType: 'blob'
    });
  }

  getExpiringLicenses(days: number = 30): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/licenses/expiring?days=${days}`);
  }
}
