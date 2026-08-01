import { Component, inject, signal, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent {
  @Input() pageTitle: string = 'لوحة التحكم';

  private readonly authService = inject(AuthService);

  readonly currentUser = this.authService.currentUser;
  readonly userRole    = this.authService.userRole;

  readonly showNotifications = signal<boolean>(false);

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
}
