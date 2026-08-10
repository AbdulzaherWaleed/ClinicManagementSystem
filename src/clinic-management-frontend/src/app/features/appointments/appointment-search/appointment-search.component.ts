import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { AppointmentService } from '../services/appointment.service';
import { AppointmentDto, CancelAppointmentCommand, GetAppointmentsQuery } from '../models/appointment.models';
import { finalize } from 'rxjs';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { DoctorService } from '../../admin/doctors/services/doctor.service';
import { DoctorDto } from '../../admin/doctors/models/doctor.models';

// PrimeNG Modules
import { TableModule, TableLazyLoadEvent, Table } from 'primeng/table';
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
import { TooltipModule } from 'primeng/tooltip';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { PermissionService } from '../../../core/auth/services/permission.service';

// نوع حالة الحجز، مأخوذ مباشرة من الـ DTO نفسها عشان نضمن التطابق
type AppointmentStatus = AppointmentDto['status'];

@Component({
  selector: 'app-appointment-search',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    FormsModule,
    RouterModule,
    TableModule, 
    SelectModule, 
    InputTextModule, 
    ButtonModule, 
    DatePickerModule,
    TagModule,
    MenuModule,
    SharedModule,
    ToastModule,
    TooltipModule,
    HasPermissionDirective
  ],
  providers: [MessageService],
  templateUrl: './appointment-search.component.html',
  styleUrls: ['./appointment-search.component.scss']
})
export class AppointmentSearchComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly appointmentService = inject(AppointmentService);
  private readonly authService = inject(AuthService);
  readonly permissionService = inject(PermissionService);
  private readonly messageService = inject(MessageService);
  private readonly doctorService = inject(DoctorService);

  readonly isAdmin = this.authService.isAdmin;

  @ViewChild('dt') table!: Table;

  searchForm!: FormGroup;
  appointments = signal<AppointmentDto[]>([]);
  doctors = signal<DoctorDto[]>([]);
  isLoading = signal<boolean>(false);
  totalRecords = signal<number>(0);

  readonly statuses = [
    { label: 'الكل', value: null },
    { label: 'مجدول', value: 'Scheduled' },
    { label: 'مكتمل (Done)', value: 'Completed' },
    { label: 'لم يحضر', value: 'NoShow' },
    { label: 'مؤجل', value: 'Postponed' },
    { label: 'ملغي', value: 'Cancelled' }
  ];

  readonly actionStatuses = [
    { label: 'مجدول', value: 'Scheduled' },
    { label: 'مكتمل (Done)', value: 'Completed' },
    { label: 'لم يحضر', value: 'NoShow' },
    { label: 'مؤجل', value: 'Postponed' },
    { label: 'ملغي', value: 'Cancelled' }
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
      doctorId: [null],
      visitStage: [null],
      status: [null],
      dateFrom: [null],
      dateTo: [null]
    });

    this.loadDoctors();
    // Load initial data (today's appointments for example, but here we just load all to show data)
    this.search();
  }

  loadDoctors(): void {
    this.doctorService.getDoctors().subscribe({
      next: (data) => this.doctors.set(data.items.filter(d => d.isActive)),
      error: (err) => console.error('Error fetching doctors', err)
    });
  }

  onLazyLoad(event: TableLazyLoadEvent): void {
    const first = event.first ?? 0;
    const rows = event.rows ?? 10;
    const pageNumber = Math.floor(first / rows) + 1;
    const pageSize = rows;
    
    this.search(pageNumber, pageSize);
  }

  search(pageNumber: number = 1, pageSize: number = 10): void {
    this.isLoading.set(true);
    const formVal = this.searchForm.value;
    
    const query: GetAppointmentsQuery & { pageNumber?: number, pageSize?: number } = {
      patientName: formVal.patientName || undefined,
      phone: formVal.phone || undefined,
      doctorId: formVal.doctorId || undefined,
      visitStage: formVal.visitStage || undefined,
      status: formVal.status || undefined,
      dateFrom: formVal.dateFrom ? new Date(formVal.dateFrom).toISOString() : undefined,
      dateTo: formVal.dateTo ? new Date(formVal.dateTo).toISOString() : undefined,
      pageNumber: pageNumber,
      pageSize: pageSize
    };

    this.appointmentService.getAppointments(query)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.appointments.set(data.items);
          this.totalRecords.set(data.totalCount);
        },
        error: (err) => console.error('Error fetching appointments', err)
      });
  }

  reset(): void {
    this.searchForm.reset();
    if (this.table) {
      this.table.first.set(0);
    }
    this.search();
  }

  getStatusLabel(status: string): string {
    const s = this.statuses.find(x => x.value === status);
    return s ? s.label : status;
  }
  
  getStageLabel(stage: string): string {
    const s = this.stages.find(x => x.value === stage);
    return s ? s.label : stage;
  }

  getStageSeverity(stage: string): 'info' | 'warn' | 'secondary' {
    switch (stage) {
      case 'Checkup': return 'info';
      case 'Consultation': return 'warn';
      case 'Procedure': return 'secondary';
      default: return 'secondary';
    }
  }

  viewDetails(appt: AppointmentDto): void {
    this.messageService.add({
      severity: 'info',
      summary: 'تفاصيل الحجز',
      detail: `${appt.patientName} — ${appt.doctorName}`
    });
  }

  onStatusChange(appt: AppointmentDto, newStatus: string): void {
    const previousStatus: AppointmentStatus = appt.status;

    if (newStatus === 'Cancelled') {
      appt.status = previousStatus; // نرجعها لحد ما نتأكد من الإلغاء
      this.cancelAppointment(appt, previousStatus);
      return;
    }

    this.changeStatus(appt, newStatus);
  }

  changeStatus(appt: AppointmentDto, newStatus: string): void {
    this.appointmentService.changeAppointmentStatus(appt.id, newStatus).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'تم', detail: 'تم تحديث الحالة بنجاح' });
        this.search();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء تحديث الحالة' });
      }
    });
  }

  exportToExcel(): void {
    const formVal = this.searchForm.value;
    
    const query: GetAppointmentsQuery = {
      patientName: formVal.patientName || undefined,
      phone: formVal.phone || undefined,
      doctorId: formVal.doctorId || undefined,
      visitStage: formVal.visitStage || undefined,
      status: formVal.status || undefined,
      dateFrom: formVal.dateFrom ? new Date(formVal.dateFrom).toISOString() : undefined,
      dateTo: formVal.dateTo ? new Date(formVal.dateTo).toISOString() : undefined
    };

    this.appointmentService.exportAppointmentsToExcel(query).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Appointments_${new Date().getTime()}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل التصدير' });
      }
    });
  }

  cancelAppointment(appt: AppointmentDto, previousStatus?: AppointmentStatus): void {
    const reason = window.prompt('سبب الإلغاء (اختياري):');

    // لو ضغط Cancel في الـ prompt (مش OK)
    if (reason === null) {
      if (previousStatus) {
        appt.status = previousStatus;
      }
      return;
    }

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
        if (previousStatus) {
          appt.status = previousStatus;
        }
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'تعذر إلغاء الحجز' });
      }
    });
  }
}