import { Component, inject, signal, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { DoctorService } from '../../../features/admin/doctors/services/doctor.service';
import { PopoverModule } from 'primeng/popover';
import { ConfigService } from '../../../core/services/config.service';
import { PermissionService } from '../../../core/auth/services/permission.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, RouterModule, PopoverModule],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent implements OnInit {
  @Input() pageTitle: string = 'لوحة التحكم';

  private readonly authService = inject(AuthService);
  private readonly doctorService = inject(DoctorService);
  readonly configService = inject(ConfigService);

  readonly currentUser = this.authService.currentUser;
  readonly userRole    = this.authService.userRole;

  readonly showNotifications = signal<boolean>(false);
  readonly isDarkMode = signal<boolean>(false);
  
  readonly expiringLicenses = signal<any[]>([]);

  private readonly permissionService = inject(PermissionService);

  ngOnInit(): void {
    if (this.userRole() === 'Admin' || (this.userRole() === 'Employee' && this.permissionService.hasPermission('Licenses.List'))) {
      if (this.configService.doctorLicensesEnabled()) {
        this.doctorService.getExpiringLicenses(30).subscribe({
          next: (licenses) => {
            this.expiringLicenses.set(licenses);
          },
          error: (err) => console.error('Error fetching expiring licenses', err)
        });
      }
    }
  }

  getRoleBadgeClass(role: string | null): string {
    const classes: Record<string, string> = {
      Admin:    'badge badge-primary',
      Doctor:   'badge badge-success',
      Employee: 'badge badge-info'
    };
    return role ? (classes[role] ?? 'badge badge-neutral') : 'badge badge-neutral';
  }

  getRoleLabel(role: string | null): string {
    const labels: Record<string, string> = {
      Admin:    'مدير',
      Doctor:   'طبيب',
      Employee: 'موظف'
    };
    return role ? (labels[role] ?? role) : '';
  }

  getCurrentDate(): string {
    return new Date().toLocaleDateString('ar-EG', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  toggleDarkMode(): void {
    this.isDarkMode.update(prev => !prev);
    const element = document.querySelector('html');
    if (this.isDarkMode()) {
      element?.classList.add('dark');
    } else {
      element?.classList.remove('dark');
    }
  }
}
