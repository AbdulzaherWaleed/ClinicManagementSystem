import { Component, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { PermissionService } from '../../../core/auth/services/permission.service';

export interface NavGroup {
  label: string;
  items: NavItem[];
}

export interface NavItem {
  label: string;
  icon: string;
  route: string;
  permission?: string | string[]; // undefined = all
  roles?: string[];         // for backward compatibility or Admin-only routes
  badge?: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  private readonly authService = inject(AuthService);
  private readonly permissionService = inject(PermissionService);

  readonly isCollapsed = signal<boolean>(false);
  readonly userRole    = this.authService.userRole;
  readonly currentUser = this.authService.currentUser;

  readonly navGroups: NavGroup[] = [
    {
      label: 'الرئيسية',
      items: [
        {
          label: 'لوحة التحكم',
          icon: 'pi pi-home',
          route: '/dashboard',
          roles: ['Admin'] // Dashboard stays admin for now, or just undefined
        }
      ]
    },
    {
      label: 'الحجوزات والمرضى',
      items: [
        {
          label: 'حجز جديد',
          icon: 'pi pi-plus-circle',
          route: '/appointments/new',
          permission: 'Bookings.Create'
        },
        {
          label: 'البحث في الحجوزات',
          icon: 'pi pi-search',
          route: '/appointments/search',
          permission: ['Bookings.List', 'Bookings.Create', 'Bookings.Edit', 'Bookings.Delete']
        },
        {
          label: 'ملفات المرضى',
          icon: 'pi pi-folder-open',
          route: '/patients',
          permission: ['Patients.List', 'Bookings.List']
        }
      ]
    },

    {
      label: 'الإدارة',
      items: [
        {
          label: 'الأطباء',
          icon: 'pi pi-user',
          route: '/doctors',
          permission: ['Doctors.List', 'Doctors.Create', 'Doctors.Edit', 'Doctors.Delete']
        },
        {
          label: 'الموظفون',
          icon: 'pi pi-users',
          route: '/employees',
          permission: ['Employees.List', 'Employees.Create', 'Employees.Edit', 'Employees.Delete']
        }
      ]
    },
    {
      label: 'التقارير والتحليلات',
      items: [
        {
          label: 'تقرير الأطباء',
          icon: 'pi pi-chart-bar',
          route: '/reports/doctors',
          permission: 'Reports.List'
        },
        {
          label: 'تقرير الموظفين',
          icon: 'pi pi-chart-line',
          route: '/reports/employees',
          permission: 'Reports.List'
        }
      ]
    },
    {
      label: 'الإعدادات',
      items: [
        {
          label: 'تخصيص العيادة',
          icon: 'pi pi-cog',
          route: '/settings',
          roles: ['Admin']
        }
      ]
    }
  ];

  /** Filter nav items based on user role */
  readonly visibleGroups = computed(() => {
    const role = this.userRole();
    return this.navGroups
      .map(group => ({
        ...group,
        items: group.items.filter(item => {
          // If neither is specified, allow access
          if (!item.roles && !item.permission) return true;
          
          // Role check
          const hasRole = item.roles ? (role && item.roles.includes(role)) : true;
          
          // Permission check
          let hasPerm = true;
          if (item.permission) {
            if (Array.isArray(item.permission)) {
              hasPerm = item.permission.some(p => this.permissionService.hasPermission(p));
            } else {
              hasPerm = this.permissionService.hasPermission(item.permission);
            }
          }
          
          return hasRole && hasPerm;
        })
      }))
      .filter(group => group.items.length > 0);
  });

  toggleCollapse(): void {
    this.isCollapsed.update(v => !v);
  }

  logout(): void {
    this.authService.logout();
  }

  getRoleLabel(role: string | null): string {
    const labels: Record<string, string> = {
      Admin: 'مدير النظام',
      Doctor: 'طبيب',
      Employee: 'موظف'
    };
    return role ? (labels[role] ?? role) : '';
  }
}
