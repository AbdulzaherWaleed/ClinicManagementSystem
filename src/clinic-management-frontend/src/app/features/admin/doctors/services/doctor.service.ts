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

  toggleStatus(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/toggle-status`, {});
  }

  deleteDoctor(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
