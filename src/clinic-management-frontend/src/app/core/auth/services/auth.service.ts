import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthResponseDto, LoginRequest, RegisterRequest, UserDto } from '../models/auth.models';

const TOKEN_KEY   = 'clinic_token';
const USER_KEY    = 'clinic_user';
const REFRESH_KEY = 'clinic_refresh';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  // --- Reactive state using Angular Signals ---
  private readonly _currentUser = signal<UserDto | null>(this.loadStoredUser());
  private readonly _isLoading   = signal<boolean>(false);

  readonly currentUser  = this._currentUser.asReadonly();
  readonly isLoading    = this._isLoading.asReadonly();
  readonly isLoggedIn   = computed(() => this._currentUser() !== null);
  readonly userRole     = computed(() => this._currentUser()?.role ?? null);
  readonly isAdmin      = computed(() => this._currentUser()?.role === 'Admin');
  readonly isDoctor     = computed(() => this._currentUser()?.role === 'Doctor');
  readonly isEmployee   = computed(() => this._currentUser()?.role === 'Employee');

  /**
   * v2.1 — Employee scoping: IDs of assigned doctors from JWT.
   * Empty for Admin and Doctor roles.
   */
  readonly assignedDoctorIds = computed(
    () => this._currentUser()?.assignedDoctorIds ?? []
  );

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router
  ) {}

  // ─────────────────────────────────────────────
  // POST /api/auth/login
  // ─────────────────────────────────────────────
  login(credentials: LoginRequest): Observable<AuthResponseDto> {
    this._isLoading.set(true);
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, credentials).pipe(
      tap({
        next: (response) => {
          this.storeSession(response);
          this._isLoading.set(false);
        },
        error: () => this._isLoading.set(false)
      })
    );
  }

  // ─────────────────────────────────────────────
  // POST /api/auth/register  (Admin only)
  // ─────────────────────────────────────────────
  register(request: RegisterRequest): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, request);
  }

  // ─────────────────────────────────────────────
  // Logout
  // ─────────────────────────────────────────────
  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(REFRESH_KEY);
    this._currentUser.set(null);
    this.router.navigate(['/auth/login']);
  }

  // ─────────────────────────────────────────────
  // Token access
  // ─────────────────────────────────────────────
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_KEY);
  }

  // ─────────────────────────────────────────────
  // Role-based checks
  // ─────────────────────────────────────────────
  hasRole(...roles: string[]): boolean {
    const role = this._currentUser()?.role;
    return role !== undefined && roles.includes(role);
  }

  /**
   * v2.1 — Employee scoping: Returns true if employee is assigned to this doctor.
   * Admin and Doctor always return true (no restriction).
   */
  canAccessDoctor(doctorId: string): boolean {
    const user = this._currentUser();
    if (!user) return false;
    if (user.role !== 'Employee') return true;
    return user.assignedDoctorIds.includes(doctorId);
  }

  // ─────────────────────────────────────────────
  // Private helpers
  // ─────────────────────────────────────────────
  private storeSession(response: AuthResponseDto): void {
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    localStorage.setItem(REFRESH_KEY, response.refreshToken);
    this._currentUser.set(response.user);
  }

  private loadStoredUser(): UserDto | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as UserDto;
    } catch {
      return null;
    }
  }
}
