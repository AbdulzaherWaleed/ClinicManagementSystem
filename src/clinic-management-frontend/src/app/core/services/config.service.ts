import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { catchError, tap } from 'rxjs/operators';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/config`;

  readonly doctorLicensesEnabled = signal<boolean>(true);

  loadFeatures() {
    return this.http.get<{ [key: string]: boolean }>(`${this.baseUrl}/features`).pipe(
      tap(features => {
        if (features && features['DoctorLicenses'] !== undefined) {
          this.doctorLicensesEnabled.set(features['DoctorLicenses']);
        }
      }),
      catchError(err => {
        console.error('Failed to load feature flags', err);
        return of(null);
      })
    );
  }
}
