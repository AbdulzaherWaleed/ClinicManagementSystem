import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreatePatientCommand, CreatedEntityResponse, PatientDto, PatientProfileDto } from '../models/patient.models';


import { PaginatedList } from '../../../core/models/pagination.models';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/patients`;

  getPatients(pageNumber: number = 1, pageSize: number = 10, searchTerm?: string): Observable<PaginatedList<PatientDto>> {
    let url = `${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) {
      url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    return this.http.get<PaginatedList<PatientDto>>(url);
  }

  createPatient(command: CreatePatientCommand): Observable<string> {
    return this.http
      .post<CreatedEntityResponse>(this.apiUrl, command)
      .pipe(map((response) => response.id));
  }

  updatePatient(id: string, command: Partial<CreatePatientCommand>): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, command);
  }

  deletePatient(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getProfile(id: string): Observable<PatientProfileDto> {
    return this.http.get<PatientProfileDto>(`${this.apiUrl}/${id}/profile`);
  }

  getPatientAppointments(id: string, pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedList<any>> {
    return this.http.get<PaginatedList<any>>(`${this.apiUrl}/${id}/appointments?page=${pageNumber}&pageSize=${pageSize}`);
  }

  getPatientDocuments(id: string, pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedList<any>> {
    return this.http.get<PaginatedList<any>>(`${this.apiUrl}/${id}/documents?page=${pageNumber}&pageSize=${pageSize}`);
  }

  uploadDocument(id: string, file: File, documentType: string, notes?: string): Observable<{ id: string }> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('documentType', documentType);
    if (notes) {
      formData.append('notes', notes);
    }
    return this.http.post<{ id: string }>(`${this.apiUrl}/${id}/documents`, formData);
  }

  downloadDocument(id: string, documentId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/documents/${documentId}/download`, {
      responseType: 'blob'
    });
  }

  deleteDocument(id: string, documentId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/documents/${documentId}`);
  }
}
