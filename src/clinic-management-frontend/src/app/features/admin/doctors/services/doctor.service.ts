import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { CreatedEntityResponse } from '../../../patients/models/patient.models';
import { DoctorDto, CreateDoctorCommand } from '../models/doctor.models';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/doctors`;

  getDoctors(): Observable<DoctorDto[]> {
    return this.http.get<DoctorDto[]>(this.apiUrl);
  }

  createDoctor(command: CreateDoctorCommand): Observable<string> {
    return this.http
      .post<CreatedEntityResponse>(this.apiUrl, command)
      .pipe(map((response) => response.id));
  }

  toggleStatus(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/toggle-status`, {});
  }
}
