import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * RoleGuard — checks user role against allowed roles defined in route data.
 *
 * Usage in routing:
 *   {
 *     path: 'employees',
 *     canActivate: [authGuard, roleGuard],
 *     data: { roles: ['Admin'] },
 *     ...
 *   }
 */
export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router      = inject(Router);

  const allowedRoles = route.data?.['roles'] as string[] | undefined;

  if (!allowedRoles || allowedRoles.length === 0) {
    return true; // No role restriction
  }

  if (authService.hasRole(...allowedRoles)) {
    return true;
  }

  // Redirect to a 403 / unauthorized page (or dashboard)
  return router.createUrlTree(['/unauthorized']);
};
