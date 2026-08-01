/**
 * Matches AuthResponseDto from the backend exactly.
 * POST /api/auth/login
 */
export interface AuthResponseDto {
  token: string;
  refreshToken: string;
  expiresAt: string; // ISO date string
  user: UserDto;
}

/**
 * Matches UserDto from the backend exactly.
 * v2.1 — assignedDoctorIds is List<Guid> for Employee Many-to-Many scoping.
 */
export interface UserDto {
  id: string;                                      // Guid
  fullName: string;
  email: string;
  role: 'Admin' | 'Doctor' | 'Employee';
  assignedDoctorIds: string[];                     // v2.1 — M2M doctor Guids
}

/**
 * Matches LoginCommand from the backend.
 * POST /api/auth/login
 */
export interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Matches RegisterCommand from the backend.
 * POST /api/auth/register  (Admin only)
 */
export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  role: 'Admin' | 'Doctor' | 'Employee';
  assignedDoctorIds: string[];                     // v2.1 — required for Employee
}
