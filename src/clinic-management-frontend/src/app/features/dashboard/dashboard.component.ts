import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/services/auth.service';
import { AppointmentService } from '../appointments/services/appointment.service';
import { AppointmentDto } from '../appointments/models/appointment.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-container animate-fade-in">
      <div class="page-title">لوحة التحكم</div>

      <div class="welcome-card">
        <div class="welcome-icon"><i class="pi pi-home"></i></div>
        <div class="welcome-text">
          <h2>أهلاً، {{ auth.currentUser()?.fullName }} 👋</h2>
          <p>مرحباً بك في نظام ميديكا لإدارة العيادات</p>
        </div>
        <div class="role-chip badge badge-primary">
          {{ getRoleLabel(auth.currentUser()?.role ?? null) }}
        </div>
      </div>

      <div class="stats-grid">
        <div class="stat-card" *ngFor="let s of stats()">
          <div class="stat-icon" [style.background]="s.color + '20'">
            <i [class]="s.icon" [style.color]="s.color"></i>
          </div>
          <div class="stat-body">
            <div class="stat-value">{{ s.value }}</div>
            <div class="stat-label">{{ s.label }}</div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    @use '../../../styles/variables' as *;
    @use '../../../styles/mixins' as *;

    .welcome-card {
      @include card(20px);
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 24px;
      background: linear-gradient(135deg, $primary-50, $surface);
      border: 1px solid $primary-100;
    }
    .welcome-icon {
      @include flex-center;
      width: 52px; height: 52px;
      background: linear-gradient(135deg, $primary-500, $primary-400);
      border-radius: $radius-lg;
      color: white;
      font-size: 22px;
      flex-shrink: 0;
    }
    .welcome-text h2 { font-size: $font-size-lg; margin-bottom: 4px; }
    .welcome-text p  { font-size: $font-size-sm; color: $text-secondary; }
    .role-chip       { margin-right: auto; }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
      gap: 16px;
    }
    .stat-card {
      @include card(20px);
      display: flex;
      align-items: center;
      gap: 14px;
      transition: transform $transition-fast, box-shadow $transition-fast;
      &:hover { transform: translateY(-2px); box-shadow: $shadow-md; }
    }
    .stat-icon {
      @include flex-center;
      width: 48px; height: 48px;
      border-radius: $radius-md;
      font-size: 20px;
      flex-shrink: 0;
    }
    .stat-value { font-size: $font-size-xl; font-weight: 700; color: $text-primary; }
    .stat-label { font-size: $font-size-xs; color: $text-secondary; font-weight: 500; }
  `]
})
export class DashboardComponent implements OnInit {
  readonly auth = inject(AuthService);
  private readonly appointmentService = inject(AppointmentService);

  readonly stats = signal([
    { label: 'حجوزات اليوم', value: '—', icon: 'pi pi-calendar', color: '#7c3aed' },
    { label: 'مرضى اليوم', value: '—', icon: 'pi pi-users', color: '#3b82f6' },
    { label: 'مكتملة اليوم', value: '—', icon: 'pi pi-check', color: '#10b981' },
    { label: 'ملغية اليوم', value: '—', icon: 'pi pi-times', color: '#ef4444' }
  ]);

  ngOnInit(): void {
    const start = new Date();
    start.setHours(0, 0, 0, 0);
    const end = new Date();
    end.setHours(23, 59, 59, 999);

    this.appointmentService
      .getAppointments({
        dateFrom: start.toISOString(),
        dateTo: end.toISOString()
      })
      .subscribe({
        next: (response) => this.updateStats(response.items),
        error: () => {}
      });
  }

  private updateStats(list: AppointmentDto[]): void {
    const patientIds = new Set(list.map((a) => a.patientId));
    const completed = list.filter((a) => a.status === 'Completed').length;
    const cancelled = list.filter((a) => a.status === 'Cancelled').length;

    this.stats.set([
      { label: 'حجوزات اليوم', value: String(list.length), icon: 'pi pi-calendar', color: '#7c3aed' },
      { label: 'مرضى اليوم', value: String(patientIds.size), icon: 'pi pi-users', color: '#3b82f6' },
      { label: 'مكتملة اليوم', value: String(completed), icon: 'pi pi-check', color: '#10b981' },
      { label: 'ملغية اليوم', value: String(cancelled), icon: 'pi pi-times', color: '#ef4444' }
    ]);
  }

  getRoleLabel(role: string | null): string {
    const m: Record<string, string> = { Admin: 'مدير النظام', Doctor: 'طبيب', Employee: 'موظف' };
    return role ? (m[role] ?? role) : '';
  }
}
