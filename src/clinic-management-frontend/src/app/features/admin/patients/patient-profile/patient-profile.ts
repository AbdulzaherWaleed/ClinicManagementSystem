import { Component, OnInit, inject, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { TabsModule } from 'primeng/tabs';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService, MessageService, SharedModule } from 'primeng/api';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { PatientService } from '../../../../features/patients/services/patient.service';
import { PatientProfileDto, PatientDocumentDto } from '../../../../features/patients/models/patient.models';
import { Subject, takeUntil, switchMap, of, tap, catchError } from 'rxjs';

export interface TabState<T> {
  items: T[];
  loading: boolean;
  totalCount: number;
  page: number;
  pageSize: number;
  loaded: boolean;
}

@Component({
  selector: 'app-patient-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    CardModule,
    TabsModule,
    ButtonModule,
    TableModule,
    DialogModule,
    ConfirmDialogModule,
    TagModule,
    ToastModule,
    InputTextModule,
    SelectModule,
    DatePickerModule,
    TextareaModule,
    TooltipModule,
    SharedModule,
    HasPermissionDirective,
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './patient-profile.html',
  styleUrls: ['./patient-profile.scss']
})
export class PatientProfileComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private patientService = inject(PatientService);
  private cdr = inject(ChangeDetectorRef);
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);
  private fb = inject(FormBuilder);
  private destroy$ = new Subject<void>();

  private loadAppointments$ = new Subject<{ page: number, size: number }>();
  private loadDocuments$ = new Subject<{ page: number, size: number }>();

  patientId!: string;
  profile!: PatientProfileDto;
  isLoading = true;

  // Edit dialog
  showEditDialog = false;
  isSaving = false;
  editForm!: FormGroup;

  genderOptions = [
    { label: 'ذكر', value: 'Male' },
    { label: 'أنثى', value: 'Female' }
  ];

  appointmentsState: TabState<any> = { items: [], loading: false, totalCount: 0, page: 1, pageSize: 10, loaded: false };
  documentsState: TabState<PatientDocumentDto> = { items: [], loading: false, totalCount: 0, page: 1, pageSize: 10, loaded: false };

  ngOnInit() {
    this.patientId = this.route.snapshot.paramMap.get('id')!;
    this.buildForm();
    this.loadProfile();
    this.setupLazyLoaders();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  buildForm() {
    this.editForm = this.fb.group({
      fullName: [''],
      phoneNumber: [''],
      email: [''],
      gender: [null],
      dateOfBirth: [null],
      nationalId: [''],
      address: [''],
      medicalNotes: [''],
      emergencyContactName: [''],
      emergencyContactPhone: ['']
    });
  }

  setupLazyLoaders() {
    this.loadAppointments$.pipe(
      takeUntil(this.destroy$),
      tap(() => { this.appointmentsState.loading = true; this.cdr.markForCheck(); }),
      switchMap(req =>
        this.patientService.getPatientAppointments(this.patientId, req.page, req.size).pipe(
          tap(res => {
            this.appointmentsState.items = res.items;
            this.appointmentsState.totalCount = res.totalCount;
            this.appointmentsState.page = req.page;
            this.appointmentsState.pageSize = req.size;
            this.appointmentsState.loaded = true;
            this.appointmentsState.loading = false;
            this.cdr.markForCheck();
          }),
          catchError(() => {
            this.appointmentsState.loading = false;
            this.cdr.markForCheck();
            return of(null);
          })
        )
      )
    ).subscribe();

    this.loadDocuments$.pipe(
      takeUntil(this.destroy$),
      tap(() => { this.documentsState.loading = true; this.cdr.markForCheck(); }),
      switchMap(req =>
        this.patientService.getPatientDocuments(this.patientId, req.page, req.size).pipe(
          tap(res => {
            this.documentsState.items = res.items;
            this.documentsState.totalCount = res.totalCount;
            this.documentsState.page = req.page;
            this.documentsState.pageSize = req.size;
            this.documentsState.loaded = true;
            this.documentsState.loading = false;
            this.cdr.markForCheck();
          }),
          catchError(() => {
            this.documentsState.loading = false;
            this.cdr.markForCheck();
            return of(null);
          })
        )
      )
    ).subscribe();
  }

  loadProfile() {
    this.isLoading = true;
    this.cdr.markForCheck();
    this.patientService.getProfile(this.patientId).subscribe({
      next: (data) => {
        this.profile = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل بيانات المريض' });
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Edit ───────────────────────────────────────
  openEditDialog() {
    const d = this.profile.demographics;
    this.editForm.patchValue({
      fullName: d.fullName,
      phoneNumber: d.phoneNumber ?? '',
      email: d.email ?? '',
      gender: d.gender ?? null,
      dateOfBirth: d.dateOfBirth ? new Date(d.dateOfBirth) : null,
      nationalId: d.nationalId ?? '',
      address: d.address ?? '',
      medicalNotes: d.medicalNotes ?? '',
      emergencyContactName: (d as any).emergencyContactName ?? '',
      emergencyContactPhone: (d as any).emergencyContactPhone ?? ''
    });
    this.showEditDialog = true;
    this.cdr.markForCheck();
  }

  saveEdit() {
    if (this.editForm.invalid) return;
    this.isSaving = true;
    const val = this.editForm.value;
    const payload: any = {};
    if (val.fullName) payload.fullName = val.fullName;
    if (val.phoneNumber) payload.phoneNumber = val.phoneNumber;
    if (val.email) payload.email = val.email;
    if (val.gender) payload.gender = val.gender;
    if (val.dateOfBirth) payload.dateOfBirth = val.dateOfBirth;
    if (val.nationalId) payload.nationalId = val.nationalId;
    if (val.address) payload.address = val.address;
    if (val.medicalNotes) payload.medicalNotes = val.medicalNotes;
    if (val.emergencyContactName) payload.emergencyContactName = val.emergencyContactName;
    if (val.emergencyContactPhone) payload.emergencyContactPhone = val.emergencyContactPhone;

    this.patientService.updatePatient(this.patientId, payload).subscribe({
      next: () => {
        this.showEditDialog = false;
        this.isSaving = false;
        this.messageService.add({ severity: 'success', summary: 'تم', detail: 'تم تحديث بيانات المريض بنجاح' });
        this.loadProfile();
      },
      error: () => {
        this.isSaving = false;
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل حفظ التعديلات' });
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Delete ──────────────────────────────────────
  confirmDelete() {
    this.confirmationService.confirm({
      message: `هل أنت متأكد من حذف المريض "${this.profile.demographics.fullName}"؟ لا يمكن التراجع عن هذا الإجراء.`,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم، احذف',
      rejectLabel: 'إلغاء',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.deletePatient()
    });
  }

  deletePatient() {
    this.patientService.deletePatient(this.patientId).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم حذف المريض بنجاح' });
        setTimeout(() => this.router.navigate(['/patients']), 1500);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل حذف المريض' });
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Tabs ─────────────────────────────────────────
  onTabChange(event: any) {
    const tabIndex = event;
    if (tabIndex === '1' && !this.appointmentsState.loaded) {
      this.loadAppointments$.next({ page: 1, size: 10 });
    } else if (tabIndex === '2' && !this.documentsState.loaded) {
      this.loadDocuments$.next({ page: 1, size: 10 });
    }
  }

  loadAppointmentsLazy(event: any) {
    const page = (event.first / event.rows) + 1;
    const size = event.rows;
    if (this.appointmentsState.loaded && this.appointmentsState.page === page && this.appointmentsState.pageSize === size) return;
    this.loadAppointments$.next({ page, size });
  }

  loadDocumentsLazy(event: any) {
    const page = (event.first / event.rows) + 1;
    const size = event.rows;
    if (this.documentsState.loaded && this.documentsState.page === page && this.documentsState.pageSize === size) return;
    this.loadDocuments$.next({ page, size });
  }

  // ─── Documents ────────────────────────────────────
  onUploadDocument(event: any) {
    const file = event.target.files[0] as File;
    if (!file) return;
    this.patientService.uploadDocument(this.patientId, file, 'Other').subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'تم الرفع', detail: 'تم رفع الملف بنجاح' });
        this.loadDocuments$.next({ page: 1, size: this.documentsState.pageSize });
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل رفع الملف' })
    });
  }

  
  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      'Scheduled': 'مجدول',
      'Completed': 'مكتمل (Done)',
      'NoShow': 'لم يحضر',
      'Postponed': 'مؤجل',
      'Cancelled': 'ملغي'
    };
    return map[status] || status;
  }

  downloadDocument(doc: PatientDocumentDto) {
    this.patientService.downloadDocument(this.patientId, doc.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url; a.download = doc.originalFileName; a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تنزيل الملف' })
    });
  }

  confirmDeleteDocument(doc: PatientDocumentDto) {
    this.confirmationService.confirm({
      message: `هل تريد حذف الملف "${doc.originalFileName}"؟`,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم، احذف',
      rejectLabel: 'إلغاء',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.patientService.deleteDocument(this.patientId, doc.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'تم', detail: 'تم حذف الملف' });
            this.loadDocuments$.next({ page: this.documentsState.page, size: this.documentsState.pageSize });
          },
          error: () => this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل الحذف' })
        });
      }
    });
  }

  // ─── Helpers ─────────────────────────────────────
  getStatusSeverity(status: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    const map: Record<string, any> = {
      'Completed': 'success', 'Scheduled': 'info',
      'NoShow': 'warn', 'Postponed': 'warn', 'Cancelled': 'danger'
    };
    return map[status] ?? 'secondary';
  }

  getStageSeverity(stage: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    const map: Record<string, any> = {
      'CheckIn': 'info', 'WithDoctor': 'warn',
      'Examination': 'warn', 'Completed': 'success', 'CheckOut': 'success'
    };
    return map[stage] ?? 'secondary';
  }

  get ageLabel(): string {
    if (!this.profile?.demographics?.dateOfBirth) return 'غير مسجل';
    const dob = new Date(this.profile.demographics.dateOfBirth);
    const age = new Date().getFullYear() - dob.getFullYear();
    return `${age} سنة`;
  }
}

