import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { CreatedEntityResponse } from '../../../patients/models/patient.models';
import { EmployeeDto, RegisterEmployeeCommand } from '../models/employee.models';
import { environment } from '../../../../../environments/environment';

import { PaginatedList } from '../../../../core/models/pagination.models';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/employees`;

  getEmployees(pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedList<EmployeeDto>> {
    return this.http.get<PaginatedList<EmployeeDto>>(`${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  registerEmployee(command: RegisterEmployeeCommand): Observable<string> {
    return this.http
      .post<CreatedEntityResponse>(`${this.apiUrl}/register`, command)
      .pipe(map((response) => response.id));
  }

  toggleStatus(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/toggle-status`, {});
  }

  deleteEmployee(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
