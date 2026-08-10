import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { PermissionService } from '../services/permission.service';

/**
 * RootRedirectGuard — dynamically redirects a logged-in user to their first permitted page,
 * or redirects to login if unauthenticated.
 */
export const rootRedirectGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const permissionService = inject(PermissionService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return router.createUrlTree(['/auth/login']);
  }

  // Admin goes to dashboard by default
  if (authService.isAdmin()) {
    return router.createUrlTree(['/dashboard']);
  }

  // Check permissions in priority order to determine landing page
  const hasAnyBooking = ['Bookings.List', 'Bookings.Create', 'Bookings.Edit', 'Bookings.Delete'].some(p => permissionService.hasPermission(p));
  if (hasAnyBooking || permissionService.hasPermission('Patients.List')) {
    return router.createUrlTree(['/appointments/search']);
  }
  
  const hasAnyDoctor = ['Doctors.List', 'Doctors.Create', 'Doctors.Edit', 'Doctors.Delete'].some(p => permissionService.hasPermission(p));
  if (hasAnyDoctor) {
    return router.createUrlTree(['/doctors']);
  }

  const hasAnyEmployee = ['Employees.List', 'Employees.Create', 'Employees.Edit', 'Employees.Delete'].some(p => permissionService.hasPermission(p));
  if (hasAnyEmployee) {
    return router.createUrlTree(['/employees']);
  }

  if (permissionService.hasPermission('Reports.List')) {
    return router.createUrlTree(['/reports/doctors']);
  }

  // If authenticated but has no navigation permissions, go to unauthorized page
  return router.createUrlTree(['/unauthorized']);
};
