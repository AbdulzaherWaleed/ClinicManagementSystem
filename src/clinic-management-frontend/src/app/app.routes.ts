import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth.guard';
import { roleGuard } from './core/auth/guards/role.guard';
import { permissionGuard } from './core/auth/guards/permission.guard';
import { rootRedirectGuard } from './core/auth/guards/root-redirect.guard';
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
        path: 'patients',
        children: [
          {
            path: '',
            canActivate: [permissionGuard],
            data: { permission: ['Patients.List', 'Bookings.List'] },
            loadComponent: () =>
              import('./features/admin/patients/patients-list/patients-list').then(m => m.PatientsListComponent)
          },
          {
            path: ':id/profile',
            canActivate: [permissionGuard],
            data: { permission: ['Patients.List', 'Bookings.List'] },
            loadComponent: () =>
              import('./features/admin/patients/patient-profile/patient-profile').then(m => m.PatientProfileComponent)
          }
        ]
      },
      {
        path: 'appointments',
        children: [
          {
            path: 'new',
            canActivate: [permissionGuard],
            data: { permission: 'Bookings.Create' },
            loadComponent: () =>
              import('./features/appointments/new-booking/new-booking.component').then(m => m.NewBookingComponent)
          },
          {
            path: 'search',
            canActivate: [permissionGuard],
            data: { permission: ['Bookings.List', 'Bookings.Create', 'Bookings.Edit', 'Bookings.Delete'] },
            loadComponent: () =>
              import('./features/appointments/appointment-search/appointment-search.component').then(m => m.AppointmentSearchComponent)
          },
          { path: '', redirectTo: 'search', pathMatch: 'full' }
        ]
      },

      // Doctors (Requires any Doctors permission or Admin)
      {
        path: 'doctors',
        canActivate: [permissionGuard],
        data: { permission: ['Doctors.List', 'Doctors.Create', 'Doctors.Edit', 'Doctors.Delete'] },
        loadComponent: () =>
          import('./features/admin/doctors/doctors.component').then(m => m.DoctorsComponent)
      },

      // Employees (Requires any Employees permission or Admin)
      {
        path: 'employees',
        canActivate: [permissionGuard],
        data: { permission: ['Employees.List', 'Employees.Create', 'Employees.Edit', 'Employees.Delete'] },
        loadComponent: () =>
          import('./features/admin/employees/employees.component').then(m => m.EmployeesComponent)
      },

      // Reports
      {
        path: 'reports',
        children: [
          {
            path: 'doctors',
            canActivate: [permissionGuard],
            data: { permission: 'Reports.List' },
            loadComponent: () =>
              import('./features/admin/reports/doctor-report/doctor-report').then(m => m.DoctorReport)
          },
          {
            path: 'employees',
            canActivate: [permissionGuard],
            data: { permission: 'Reports.List' },
            loadComponent: () =>
              import('./features/admin/reports/staff-report/staff-report').then(m => m.StaffReport)
          },
          { path: '', redirectTo: 'doctors', pathMatch: 'full' }
        ]
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
      { path: '', canActivate: [rootRedirectGuard], children: [] }
    ]
  },

  // Catch-all
  { path: '**', redirectTo: '/appointments/search' }
];
