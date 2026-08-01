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
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { SharedModule } from 'primeng/api';

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
    SharedModule
  ],
  providers: [MessageService],
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.scss']
})
export class DoctorsComponent implements OnInit {
  private readonly doctorService = inject(DoctorService);
  private readonly fb = inject(FormBuilder);
  private readonly messageService = inject(MessageService);

  doctors = signal<DoctorDto[]>([]);
  isLoading = signal<boolean>(false);
  
  displayAddDialog = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  addForm!: FormGroup;

  ngOnInit(): void {
    this.initForm();
    this.loadDoctors();
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

  loadDoctors(): void {
    this.isLoading.set(true);
    this.doctorService.getDoctors()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => this.doctors.set(data),
        error: () => this.showError('حدث خطأ أثناء تحميل الأطباء')
      });
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

  toggleStatus(doctor: DoctorDto): void {
    this.doctorService.toggleStatus(doctor.id).subscribe({
      next: () => {
        doctor.isActive = !doctor.isActive;
        this.showSuccess('تم تحديث حالة الطبيب');
      },
      error: () => this.showError('حدث خطأ أثناء تغيير الحالة')
    });
  }

  private showSuccess(msg: string): void {
    this.messageService.add({ severity: 'success', summary: 'نجاح', detail: msg });
  }

  private showError(msg: string): void {
    this.messageService.add({ severity: 'error', summary: 'خطأ', detail: msg });
  }
}
