import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

/**
 * JWT Interceptor — attaches Authorization: Bearer {token} to all API requests.
 * Skips /auth/login and /auth/register endpoints.
 */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  // Skip auth endpoints
  const isAuthEndpoint =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register');

  if (isAuthEndpoint) {
    return next(req);
  }

  const token = authService.getToken();

  if (token) {
    const cloned = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(cloned);
  }

  return next(req);
};
