import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs';
import { getDirtyValues } from '../../../shared/utils/form-utils';

import { EmployeeService } from './services/employee.service';
import { EmployeeDto } from './models/employee.models';
import { DoctorService } from '../doctors/services/doctor.service';
import { DoctorDto } from '../doctors/models/doctor.models';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { PasswordModule } from 'primeng/password';
import { MultiSelectModule } from 'primeng/multiselect';
import { SelectModule } from 'primeng/select';
import { ScrollerModule } from 'primeng/scroller';
import { SharedModule } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { PermissionService } from '../../../core/auth/services/permission.service';

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
    SelectModule,
    ScrollerModule,
    SharedModule,
    ConfirmDialogModule,
    TooltipModule,
    HasPermissionDirective
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss']
})
export class EmployeesComponent implements OnInit {
  private readonly employeeService = inject(EmployeeService);
  private readonly doctorService = inject(DoctorService);
  private readonly fb = inject(FormBuilder);
  private readonly messageService = inject(MessageService);
  private readonly confirmationService = inject(ConfirmationService);
  readonly permissionService = inject(PermissionService);

  employees = signal<EmployeeDto[]>([]);
  totalRecords = signal<number>(0);
  doctors = signal<DoctorDto[]>([]);
  
  isLoading = signal<boolean>(false);
  displayAddDialog = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  addForm!: FormGroup;

  displayPermissionsDialog = signal<boolean>(false);
  isSavingPermissions = signal<boolean>(false);
  selectedEmployeeId = signal<string | null>(null);
  selectedEmployeeName = signal<string>('');
  
  // Available Permissions
  readonly availablePermissions = [
    { module: 'الحجوزات', items: [{ label: 'عرض', value: 'Bookings.List' }, { label: 'إضافة', value: 'Bookings.Create' }, { label: 'تعديل', value: 'Bookings.Edit' }, { label: 'حذف', value: 'Bookings.Delete' }] },
    { module: 'الأطباء', items: [{ label: 'عرض', value: 'Doctors.List' }, { label: 'إضافة', value: 'Doctors.Create' }, { label: 'تعديل', value: 'Doctors.Edit' }, { label: 'حذف', value: 'Doctors.Delete' }] },
    { module: 'تراخيص الأطباء', items: [{ label: 'عرض وتحميل', value: 'Licenses.List' }, { label: 'رفع', value: 'Licenses.Create' }] },
    { module: 'المرضى', items: [{ label: 'عرض', value: 'Patients.List' }, { label: 'إضافة', value: 'Patients.Create' }, { label: 'تعديل', value: 'Patients.Edit' }, { label: 'حذف', value: 'Patients.Delete' }] },
    { module: 'الموظفين', items: [{ label: 'عرض', value: 'Employees.List' }, { label: 'إضافة', value: 'Employees.Create' }, { label: 'تعديل', value: 'Employees.Edit' }, { label: 'حذف', value: 'Employees.Delete' }] },
    { module: 'التقارير', items: [{ label: 'عرض', value: 'Reports.List' }] }
  ];

  selectedPermissions = signal<string[]>([]);

  // Edit employee dialog
  displayEditDialog = signal<boolean>(false);
  isUpdating = signal<boolean>(false);
  editingEmployee = signal<EmployeeDto | null>(null);
  editForm!: FormGroup;

  ngOnInit(): void {
    this.initForm();
    // Initial load will be triggered by onLazyLoad event of p-table
    this.loadDoctors();
  }

  private initForm(): void {
    this.addForm = this.fb.group({
      fullName: [null, Validators.required],
      email: [null, [Validators.required, Validators.email]],
      password: [null, [Validators.required, Validators.minLength(8)]],
      phoneNumber: [null],
      jobTitle: [null],
      assignedDoctorIds: [[], Validators.required]
    });

    this.editForm = this.fb.group({
      fullName: [null, Validators.required],
      phoneNumber: [null],
      jobTitle: [null]
    });
  }

  showEditEmployee(employee: EmployeeDto): void {
    this.editingEmployee.set(employee);
    this.editForm.reset();
    this.editForm.patchValue({
      fullName: employee.fullName,
      phoneNumber: employee.phoneNumber,
      jobTitle: employee.jobTitle
    });
    this.displayEditDialog.set(true);
  }

  saveEditEmployee(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }
    const emp = this.editingEmployee();
    if (!emp) return;

    const patch = getDirtyValues(this.editForm);
    if (Object.keys(patch).length === 0) {
      this.displayEditDialog.set(false);
      return;
    }

    this.isUpdating.set(true);
    this.employeeService.updateEmployee(emp.id, patch)
      .pipe(finalize(() => this.isUpdating.set(false)))
      .subscribe({
        next: () => {
          this.displayEditDialog.set(false);
          this.showSuccess('تم تحديث بيانات الموظف بنجاح');
          this.loadEmployees();
        },
        error: () => this.showError('حدث خطأ أثناء تحديث بيانات الموظف')
      });
  }

  loadEmployees(pageNumber: number = 1, pageSize: number = 10): void {
    this.isLoading.set(true);
    this.employeeService.getEmployees(pageNumber, pageSize)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.employees.set(data.items);
          this.totalRecords.set(data.totalCount);
        },
        error: () => this.showError('حدث خطأ أثناء تحميل الموظفين')
      });
  }

  onLazyLoad(event: any): void {
    const pageNumber = (event.first ?? 0) / (event.rows ?? 10) + 1;
    const pageSize = event.rows ?? 10;
    this.loadEmployees(pageNumber, pageSize);
  }

  loadDoctors(): void {
    // Only fetch active doctors for assignment
    this.doctorService.getDoctors().subscribe({
      next: (data) => this.doctors.set(data.items.filter(d => d.isActive)),
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

  onDoctorCheckboxChange(event: any, doctorId: string): void {
    const assignedDocsControl = this.addForm.get('assignedDoctorIds');
    if (!assignedDocsControl) return;
    
    let currentIds = (assignedDocsControl.value as string[]) || [];
    if (event.target.checked) {
      if (!currentIds.includes(doctorId)) {
        currentIds.push(doctorId);
      }
    } else {
      currentIds = currentIds.filter(id => id !== doctorId);
    }
    
    assignedDocsControl.setValue(currentIds);
    assignedDocsControl.markAsDirty();
    assignedDocsControl.updateValueAndValidity();
  }

  isDoctorAssigned(doctorId: string): boolean {
    const assignedDocsControl = this.addForm.get('assignedDoctorIds');
    if (!assignedDocsControl) return false;
    const currentIds = (assignedDocsControl.value as string[]) || [];
    return currentIds.includes(doctorId);
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

  deleteEmployee(employee: EmployeeDto): void {
    this.confirmationService.confirm({
      message: `هل أنت متأكد من حذف الموظف ${employee.fullName}؟`,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      accept: () => {
        this.employeeService.deleteEmployee(employee.id).subscribe({
          next: () => {
            this.showSuccess('تم حذف الموظف بنجاح');
            this.loadEmployees();
          },
          error: () => this.showError('حدث خطأ أثناء الحذف')
        });
      }
    });
  }

  showPermissionsDialog(employee: EmployeeDto): void {
    this.selectedEmployeeId.set(employee.id);
    this.selectedEmployeeName.set(employee.fullName);
    this.selectedPermissions.set([]);
    this.displayPermissionsDialog.set(true);

    this.employeeService.getPermissions(employee.id).subscribe({
      next: (data) => {
        this.selectedPermissions.set(data.permissions || []);
      },
      error: () => this.showError('حدث خطأ أثناء جلب الصلاحيات')
    });
  }

  onPermissionCheckboxChange(event: any, permissionValue: string): void {
    const isChecked = event.target.checked;
    let current = [...this.selectedPermissions()];
    if (isChecked) {
      if (!current.includes(permissionValue)) current.push(permissionValue);
    } else {
      current = current.filter(p => p !== permissionValue);
    }
    this.selectedPermissions.set(current);
  }

  savePermissions(): void {
    const empId = this.selectedEmployeeId();
    if (!empId) return;

    this.isSavingPermissions.set(true);
    this.employeeService.updatePermissions(empId, { permissions: this.selectedPermissions() })
      .pipe(finalize(() => this.isSavingPermissions.set(false)))
      .subscribe({
        next: () => {
          this.displayPermissionsDialog.set(false);
          this.showSuccess('تم تحديث الصلاحيات بنجاح');
        },
        error: (err) => {
          const msg = err.error?.title || 'حدث خطأ أثناء حفظ الصلاحيات';
          this.showError(msg);
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
