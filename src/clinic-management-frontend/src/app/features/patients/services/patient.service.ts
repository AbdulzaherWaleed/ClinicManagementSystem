import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreatePatientCommand, CreatedEntityResponse, PatientDto } from '../models/patient.models';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/patients`;

  getPatients(): Observable<PatientDto[]> {
    return this.http.get<PatientDto[]>(this.apiUrl);
  }

  createPatient(command: CreatePatientCommand): Observable<string> {
    return this.http
      .post<CreatedEntityResponse>(this.apiUrl, command)
      .pipe(map((response) => response.id));
  }
}
