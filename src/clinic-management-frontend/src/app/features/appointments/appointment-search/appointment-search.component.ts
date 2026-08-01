import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { AppointmentService } from '../services/appointment.service';
import { AppointmentDto, CancelAppointmentCommand, GetAppointmentsQuery } from '../models/appointment.models';
import { finalize } from 'rxjs';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';

// PrimeNG Modules
import { TableModule } from 'primeng/table';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { TagModule } from 'primeng/tag';
import { MenuModule } from 'primeng/menu';
import { MenuItem, SharedModule } from 'primeng/api';
import { Menu } from 'primeng/menu';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-appointment-search',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    RouterModule,
    TableModule, 
    SelectModule, 
    InputTextModule, 
    ButtonModule, 
    DatePickerModule,
    TagModule,
    MenuModule,
    SharedModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './appointment-search.component.html',
  styleUrls: ['./appointment-search.component.scss']
})
export class AppointmentSearchComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly appointmentService = inject(AppointmentService);
  private readonly authService = inject(AuthService);
  private readonly messageService = inject(MessageService);

  readonly isAdmin = this.authService.isAdmin;

  searchForm!: FormGroup;
  appointments = signal<AppointmentDto[]>([]);
  isLoading = signal<boolean>(false);
  actionMenuItems = signal<MenuItem[]>([]);

  readonly statuses = [
    { label: 'الكل', value: null },
    { label: 'مجدول', value: 'Scheduled' },
    { label: 'مؤكد', value: 'Confirmed' },
    { label: 'في الانتظار', value: 'CheckedIn' },
    { label: 'قيد الكشف', value: 'InProgress' },
    { label: 'مكتمل', value: 'Completed' },
    { label: 'ملغي', value: 'Cancelled' },
    { label: 'لم يحضر', value: 'NoShow' }
  ];

  readonly stages = [
    { label: 'الكل', value: null },
    { label: 'كشف', value: 'Checkup' },
    { label: 'استشارة', value: 'Consultation' },
    { label: 'إجراء / جراحة', value: 'Procedure' }
  ];

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      patientName: [null],
      phone: [null],
      visitStage: [null],
      status: [null],
      dateFrom: [null],
      dateTo: [null]
    });

    // Load initial data (today's appointments for example, but here we just load all to show data)
    this.search();
  }

  search(): void {
    this.isLoading.set(true);
    const formVal = this.searchForm.value;
    
    const query: GetAppointmentsQuery = {
      patientName: formVal.patientName || undefined,
      phone: formVal.phone || undefined,
      visitStage: formVal.visitStage || undefined,
      status: formVal.status || undefined,
      dateFrom: formVal.dateFrom ? new Date(formVal.dateFrom).toISOString() : undefined,
      dateTo: formVal.dateTo ? new Date(formVal.dateTo).toISOString() : undefined
    };

    this.appointmentService.getAppointments(query)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => this.appointments.set(data),
        error: (err) => console.error('Error fetching appointments', err)
      });
  }

  reset(): void {
    this.searchForm.reset();
    this.search();
  }

  getSeverity(status: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    switch (status) {
      case 'Scheduled': return 'info';
      case 'Confirmed': return 'success';
      case 'CheckedIn': return 'warn';
      case 'InProgress': return 'warn';
      case 'Completed': return 'success';
      case 'Cancelled': return 'danger';
      case 'NoShow': return 'secondary';
      default: return 'info';
    }
  }

  getStatusLabel(status: string): string {
    const s = this.statuses.find(x => x.value === status);
    return s ? s.label : status;
  }
  
  getStageLabel(stage: string): string {
    const s = this.stages.find(x => x.value === stage);
    return s ? s.label : stage;
  }

  getMenuItems(appt: AppointmentDto): MenuItem[] {
    const items: MenuItem[] = [
      {
        label: 'عرض التفاصيل',
        icon: 'pi pi-eye',
        command: () => this.messageService.add({
          severity: 'info',
          summary: 'تفاصيل الحجز',
          detail: `${appt.patientName} — ${appt.doctorName}`
        })
      }
    ];

    if (appt.status !== 'Cancelled' && appt.status !== 'Completed') {
      items.push({
        label: 'إلغاء الحجز',
        icon: 'pi pi-times',
        styleClass: 'text-red-500',
        command: () => this.cancelAppointment(appt)
      });
    }

    return items;
  }

  openActionsMenu(event: Event, appt: AppointmentDto, menu: Menu): void {
    this.actionMenuItems.set(this.getMenuItems(appt));
    menu.toggle(event);
  }

  private cancelAppointment(appt: AppointmentDto): void {
    const reason = window.prompt('سبب الإلغاء (اختياري):') ?? undefined;
    if (reason === null) return;

    const command: CancelAppointmentCommand = {
      id: appt.id,
      cancellationReason: reason || undefined
    };

    this.appointmentService.cancelAppointment(appt.id, command).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'تم', detail: 'تم إلغاء الحجز' });
        this.search();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'تعذر إلغاء الحجز' });
      }
    });
  }
}
