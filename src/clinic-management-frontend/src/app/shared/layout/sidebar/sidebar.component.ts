import { Component, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';

export interface NavGroup {
  label: string;
  items: NavItem[];
}

export interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles?: string[];         // undefined = all roles
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
          route: '/dashboard'
        }
      ]
    },
    {
      label: 'الحجوزات والمرضى',
      items: [
        {
          label: 'حجز جديد',
          icon: 'pi pi-plus-circle',
          route: '/appointments/new'
        },
        {
          label: 'البحث في الحجوزات',
          icon: 'pi pi-search',
          route: '/appointments/search'
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
          roles: ['Admin']
        },
        {
          label: 'الموظفون',
          icon: 'pi pi-users',
          route: '/employees',
          roles: ['Admin']
        }
      ]
    },
    {
      label: 'التقارير والتحليلات',
      items: [
        {
          label: 'تقرير الأطباء',
          icon: 'pi pi-chart-bar',
          route: '/reports/doctors'
        },
        {
          label: 'تقرير الموظفين',
          icon: 'pi pi-chart-line',
          route: '/reports/employees',
          roles: ['Admin']
        }
      ]
    },
    {
      label: 'الإعدادات',
      items: [
        {
          label: 'قسم التراخيص',
          icon: 'pi pi-file-pdf',
          route: '/licenses',
          roles: ['Admin', 'Doctor']
        },
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
        items: group.items.filter(item =>
          !item.roles || (role && item.roles.includes(role))
        )
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
