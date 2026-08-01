import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs';

import { EmployeeService } from './services/employee.service';
import { EmployeeDto } from './models/employee.models';
import { DoctorService } from '../doctors/services/doctor.service';
import { DoctorDto } from '../doctors/models/doctor.models';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { PasswordModule } from 'primeng/password';
import { MultiSelectModule } from 'primeng/multiselect';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-employees',
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
    MultiSelectModule,
    PasswordModule,
    SelectModule
  ],
  providers: [MessageService],
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss']
})
export class EmployeesComponent implements OnInit {
  private readonly employeeService = inject(EmployeeService);
  private readonly doctorService = inject(DoctorService);
  private readonly fb = inject(FormBuilder);
  private readonly messageService = inject(MessageService);

  employees = signal<EmployeeDto[]>([]);
  doctors = signal<DoctorDto[]>([]);
  
  isLoading = signal<boolean>(false);
  displayAddDialog = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  addForm!: FormGroup;

  ngOnInit(): void {
    this.initForm();
    this.loadEmployees();
    this.loadDoctors();
  }

  private initForm(): void {
    this.addForm = this.fb.group({
      fullName: [null, Validators.required],
      email: [null, [Validators.required, Validators.email]],
      password: [null, [Validators.required, Validators.minLength(8)]],
      phoneNumber: [null],
      jobTitle: [null],
      assignedDoctorIds: [[]]
    });
  }

  loadEmployees(): void {
    this.isLoading.set(true);
    this.employeeService.getEmployees()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => this.employees.set(data),
        error: () => this.showError('حدث خطأ أثناء تحميل الموظفين')
      });
  }

  loadDoctors(): void {
    // Only fetch active doctors for assignment
    this.doctorService.getDoctors().subscribe({
      next: (data) => this.doctors.set(data.filter(d => d.isActive)),
      error: () => this.showError('تعذر تحميل الأطباء')
    });
  }

  showAddDialog(): void {
    this.addForm.reset();
    this.displayAddDialog.set(true);
  }

  saveEmployee(): void {
    if (this.addForm.invalid) {
      this.addForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.employeeService.registerEmployee(this.addForm.value)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.displayAddDialog.set(false);
          this.showSuccess('تمت إضافة الموظف بنجاح');
          this.loadEmployees();
        },
        error: (err) => {
          // Display validation errors from backend if any
          const msg = err.error?.errors ? Object.values(err.error.errors).flat().join(', ') : 'حدث خطأ أثناء حفظ بيانات الموظف';
          this.showError(msg);
        }
      });
  }

  toggleStatus(employee: EmployeeDto): void {
    this.employeeService.toggleStatus(employee.id).subscribe({
      next: () => {
        employee.isActive = !employee.isActive;
        this.showSuccess('تم تحديث حالة الموظف');
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
