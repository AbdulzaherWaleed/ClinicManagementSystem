import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../services/permission.service';
import { AuthService } from '../services/auth.service';

/**
 * PermissionGuard — checks user permissions against required permission defined in route data.
 * Admin users automatically pass all checks.
 */
export const permissionGuard: CanActivateFn = (route, state) => {
  const permissionService = inject(PermissionService);
  const authService = inject(AuthService);
  const router = inject(Router);

  // Admin bypasses all permission checks
  if (authService.isAdmin()) {
    return true;
  }

  const requiredPermission = route.data?.['permission'] as string | string[] | undefined;

  if (!requiredPermission) {
    return true; // No permission restriction
  }

  if (Array.isArray(requiredPermission)) {
    const hasAny = requiredPermission.some(p => permissionService.hasPermission(p));
    if (hasAny) {
      return true;
    }
  } else {
    if (permissionService.hasPermission(requiredPermission)) {
      return true;
    }
  }

  // Redirect to a 403 / unauthorized page
  return router.createUrlTree(['/unauthorized']);
};
