import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs';

import { DoctorService } from './services/doctor.service';
import { DoctorDto } from './models/doctor.models';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { SharedModule } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

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
    ConfirmDialogModule
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

  doctors = signal<DoctorDto[]>([]);
  totalRecords = signal<number>(0);
  isLoading = signal<boolean>(false);
  
  displayAddDialog = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  addForm!: FormGroup;

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
    const pageNumber = (event.first / event.rows) + 1;
    const pageSize = event.rows || 10;
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
          this.loadDoctors(); // load page 1 after adding
        },
        error: () => this.showError('حدث خطأ أثناء حفظ بيانات الطبيب')
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
}
