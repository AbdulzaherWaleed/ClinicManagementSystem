import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth.guard';
import { roleGuard } from './core/auth/guards/role.guard';
import { MainLayoutComponent } from './shared/layout/main-layout/main-layout.component';

export const routes: Routes = [

  // ─── Public Routes (no guard) ───────────────────────────
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login.component').then(m => m.LoginComponent)
      },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },

  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./features/auth/unauthorized/unauthorized.component')
        .then(m => m.UnauthorizedComponent)
  },

  // ─── Protected Routes (inside Main Layout shell) ─────────
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [

      // Dashboard (Admin only)
      {
        path: 'dashboard',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },

      // Appointments & Patients
      {
        path: 'appointments',
        children: [
          {
            path: 'new',
            loadComponent: () =>
              import('./features/appointments/new-booking/new-booking.component').then(m => m.NewBookingComponent)
          },
          {
            path: 'search',
            loadComponent: () =>
              import('./features/appointments/appointment-search/appointment-search.component').then(m => m.AppointmentSearchComponent)
          },
          { path: '', redirectTo: 'search', pathMatch: 'full' }
        ]
      },

      // Doctors (Admin only)
      {
        path: 'doctors',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () =>
          import('./features/admin/doctors/doctors.component').then(m => m.DoctorsComponent)
      },

      // Employees (Admin only)
      {
        path: 'employees',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () =>
          import('./features/admin/employees/employees.component').then(m => m.EmployeesComponent)
      },

      // Reports
      {
        path: 'reports',
        children: [
          {
            path: 'doctors',
            canActivate: [roleGuard],
            data: { roles: ['Admin'] },
            loadComponent: () =>
              import('./features/admin/reports/doctor-report/doctor-report').then(m => m.DoctorReport)
          },
          {
            path: 'employees',
            canActivate: [roleGuard],
            data: { roles: ['Admin'] },
            loadComponent: () =>
              import('./features/admin/reports/staff-report/staff-report').then(m => m.StaffReport)
          },
          { path: '', redirectTo: 'doctors', pathMatch: 'full' }
        ]
      },

      // Licenses (Admin + Doctor)
      {
        path: 'licenses',
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Doctor'],
          title: 'قسم التراخيص',
          description: 'إدارة تراخيص الأطباء والمستندات ستتوفر هنا قريباً.'
        },
        loadComponent: () =>
          import('./shared/components/feature-placeholder/feature-placeholder.component')
            .then(m => m.FeaturePlaceholderComponent)
      },

      // Settings (Admin only)
      {
        path: 'settings',
        canActivate: [roleGuard],
        data: {
          roles: ['Admin'],
          title: 'تخصيص العيادة',
          description: 'إعدادات العيادة والتخصيص ستتوفر هنا قريباً.'
        },
        loadComponent: () =>
          import('./shared/components/feature-placeholder/feature-placeholder.component')
            .then(m => m.FeaturePlaceholderComponent)
      },

      // Default redirect
      { path: '', redirectTo: 'appointments/search', pathMatch: 'full' }
    ]
  },

  // Catch-all
  { path: '**', redirectTo: '/appointments/search' }
];
