import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs';
import { getDirtyValues } from '../../../shared/utils/form-utils';

import { DoctorService } from './services/doctor.service';
import { DoctorDto } from './models/doctor.models';
import { environment } from '../../../../environments/environment';
import { ConfigService } from '../../../core/services/config.service';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { SharedModule } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { PermissionService } from '../../../core/auth/services/permission.service';

@Component({
  selector: 'app-doctors',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    TagModule,
    ToastModule,
    SharedModule,
    ConfirmDialogModule,
    TooltipModule,
    HasPermissionDirective
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.scss']
})
export class DoctorsComponent implements OnInit {
  private readonly doctorService = inject(DoctorService);
  private readonly fb = inject(FormBuilder);
  private readonly messageService = inject(MessageService);
  private readonly confirmationService = inject(ConfirmationService);
  readonly configService = inject(ConfigService);
  readonly permissionService = inject(PermissionService);

  doctors = signal<DoctorDto[]>([]);
  totalRecords = signal<number>(0);
  isLoading = signal<boolean>(false);

  displayAddDialog = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  addForm!: FormGroup;

  // Edit dialog state
  displayEditDialog = signal<boolean>(false);
  isUpdating = signal<boolean>(false);
  editForm!: FormGroup;
  editingDoctor = signal<DoctorDto | null>(null);

  ngOnInit(): void {
    this.initForm();
    // Intentionally omitting this.loadDoctors() here. 
    // PrimeNG p-table with [lazy]="true" will trigger onLazyLoad initially.
  }

  private initForm(): void {
    this.addForm = this.fb.group({
      fullName: [null, Validators.required],
      title: [null],
      bio: [null],
      phoneNumber: [null],
      email: [null, Validators.email]
    });

    this.editForm = this.fb.group({
      fullName: [null, Validators.required],
      title: [null],
      bio: [null],
      phoneNumber: [null],
      email: [null, Validators.email]
    });
  }

  loadDoctors(pageNumber: number = 1, pageSize: number = 10): void {
    this.isLoading.set(true);
    this.doctorService.getDoctors(pageNumber, pageSize)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.doctors.set(data.items);
          this.totalRecords.set(data.totalCount);
        },
        error: () => this.showError('حدث خطأ أثناء تحميل الأطباء')
      });
  }

  onLazyLoad(event: any): void {
    const pageNumber = (event.first ?? 0) / (event.rows ?? 10) + 1;
    const pageSize = event.rows ?? 10;
    this.loadDoctors(pageNumber, pageSize);
  }

  showAddDialog(): void {
    this.addForm.reset();
    this.displayAddDialog.set(true);
  }

  saveDoctor(): void {
    if (this.addForm.invalid) {
      this.addForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.doctorService.createDoctor(this.addForm.value)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.displayAddDialog.set(false);
          this.showSuccess('تمت إضافة الطبيب بنجاح');
          this.loadDoctors();
        },
        error: () => this.showError('حدث خطأ أثناء حفظ بيانات الطبيب')
      });
  }

  showEditDialog(doctor: DoctorDto): void {
    this.editingDoctor.set(doctor);
    this.editForm.reset();
    this.editForm.patchValue({
      fullName: doctor.fullName,
      title: doctor.title,
      bio: doctor.bio,
      phoneNumber: doctor.phoneNumber,
      email: doctor.email
    });
    this.displayEditDialog.set(true);
  }

  saveEditDoctor(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }
    const doctor = this.editingDoctor();
    if (!doctor) return;

    const patch = getDirtyValues(this.editForm);
    if (Object.keys(patch).length === 0) {
      this.displayEditDialog.set(false);
      return;
    }

    this.isUpdating.set(true);
    this.doctorService.updateDoctor(doctor.id, patch)
      .pipe(finalize(() => this.isUpdating.set(false)))
      .subscribe({
        next: () => {
          this.displayEditDialog.set(false);
          this.showSuccess('تم تحديث بيانات الطبيب بنجاح');
          this.loadDoctors();
        },
        error: () => this.showError('حدث خطأ أثناء تحديث بيانات الطبيب')
      });
  }

  toggleStatus(doctor: DoctorDto): void {
    this.doctorService.toggleStatus(doctor.id).subscribe({
      next: () => {
        doctor.isActive = !doctor.isActive;
        this.showSuccess('تم تحديث حالة الطبيب');
      },
      error: () => this.showError('حدث خطأ أثناء تغيير الحالة')
    });
  }

  deleteDoctor(doctor: DoctorDto): void {
    this.confirmationService.confirm({
      message: `هل أنت متأكد من حذف الطبيب ${doctor.fullName}؟`,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      accept: () => {
        this.doctorService.deleteDoctor(doctor.id).subscribe({
          next: () => {
            this.showSuccess('تم حذف الطبيب بنجاح');
            this.loadDoctors();
          },
          error: () => this.showError('حدث خطأ أثناء الحذف')
        });
      }
    });
  }

  private showSuccess(msg: string): void {
    this.messageService.add({ severity: 'success', summary: 'نجاح', detail: msg });
  }

  private showError(msg: string): void {
    this.messageService.add({ severity: 'error', summary: 'خطأ', detail: msg });
  }

  // License Management
  displayLicensesDialog = signal<boolean>(false);
  displayUploadLicenseDialog = signal<boolean>(false);
  selectedDoctorForLicense = signal<DoctorDto | null>(null);
  doctorLicenses = signal<any[]>([]);
  uploadLicenseForm!: FormGroup;
  selectedFile: File | null = null;
  isUploadingLicense = signal<boolean>(false);

  private initLicenseForm(): void {
    this.uploadLicenseForm = this.fb.group({
      licenseNumber: [null],
      expiryDate: [null, Validators.required]
    });
  }

  showLicenses(doctor: DoctorDto): void {
    this.selectedDoctorForLicense.set(doctor);
    this.loadDoctorLicenses(doctor.id);
    this.displayLicensesDialog.set(true);
  }

  loadDoctorLicenses(doctorId: string): void {
    this.doctorService.getLicenses(doctorId).subscribe({
      next: (data) => this.doctorLicenses.set(data),
      error: () => this.showError('تعذر تحميل تراخيص الطبيب')
    });
  }

  showUploadLicense(doctor: DoctorDto): void {
    if (!this.uploadLicenseForm) this.initLicenseForm();
    this.uploadLicenseForm.reset();
    this.selectedFile = null;
    this.selectedDoctorForLicense.set(doctor);
    this.displayUploadLicenseDialog.set(true);
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) {
        this.showError('حجم الملف يجب ألا يتجاوز 5 ميجابايت');
        this.selectedFile = null;
        event.target.value = ''; // Reset input
        return;
      }
      this.selectedFile = file;
    }
  }

  uploadLicense(): void {
    if (this.uploadLicenseForm.invalid) {
      this.uploadLicenseForm.markAllAsTouched();
      this.showError('يرجى تعبئة جميع الحقول المطلوبة');
      return;
    }
    if (!this.selectedFile) {
      this.showError('يجب اختيار ملف الترخيص');
      return;
    }
    const doc = this.selectedDoctorForLicense();
    if (!doc) return;

    this.isUploadingLicense.set(true);
    const formVals = this.uploadLicenseForm.value;
    
    this.doctorService.uploadLicense(doc.id, this.selectedFile, formVals.licenseNumber, formVals.expiryDate)
      .pipe(finalize(() => this.isUploadingLicense.set(false)))
      .subscribe({
        next: () => {
          this.showSuccess('تم رفع الترخيص بنجاح');
          this.displayUploadLicenseDialog.set(false);
          if (this.displayLicensesDialog()) {
            this.loadDoctorLicenses(doc.id);
          }
        },
        error: (err) => {
          const msg = err.error?.detail || err.error?.message || err.error || 'حدث خطأ أثناء رفع الترخيص';
          this.showError(typeof msg === 'string' ? msg : 'خطأ غير معروف');
        }
      });
  }

  viewLicenseFile(license: any): void {
    const doc = this.selectedDoctorForLicense();
    if (!doc) return;

    this.doctorService.downloadLicense(doc.id, license.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, '_blank');
      },
      error: () => this.showError('حدث خطأ أثناء تحميل الملف')
    });
  }
}
