import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AppointmentService } from '../services/appointment.service';
import { CreateAppointmentCommand } from '../models/appointment.models';
import { PatientService } from '../../patients/services/patient.service';
import { PatientDto } from '../../patients/models/patient.models';
import { DoctorService } from '../../admin/doctors/services/doctor.service';
import { DoctorDto } from '../../admin/doctors/models/doctor.models';
import { AuthService } from '../../../core/auth/services/auth.service';
import { finalize } from 'rxjs';

import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';
import { DialogModule } from 'primeng/dialog';

@Component({
  selector: 'app-new-booking',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    SelectModule,
    InputTextModule,
    ButtonModule,
    DatePickerModule,
    TextareaModule,
    TooltipModule,
    DialogModule
  ],
  templateUrl: './new-booking.component.html',
  styleUrls: ['./new-booking.component.scss']
})
export class NewBookingComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly appointmentService = inject(AppointmentService);
  private readonly patientService = inject(PatientService);
  private readonly doctorService = inject(DoctorService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  bookingForm!: FormGroup;
  patientForm!: FormGroup;

  isLoading = signal<boolean>(false);
  isLoadingLookups = signal<boolean>(true);
  isSavingPatient = signal<boolean>(false);
  serverError = signal<string>('');

  patients = signal<PatientDto[]>([]);
  allDoctors = signal<DoctorDto[]>([]);
  filteredDoctors = signal<DoctorDto[]>([]);
  specialties = signal<{ label: string; value: string }[]>([]);

  displayPatientDialog = signal<boolean>(false);

  readonly stages = [
    { label: 'كشف', value: 'Checkup' },
    { label: 'استشارة', value: 'Consultation' },
    { label: 'إجراء / جراحة', value: 'Procedure' }
  ];

  ngOnInit(): void {
    this.bookingForm = this.fb.group({
      patientId: [null, Validators.required],
      specialty: [null],
      doctorId: [null, Validators.required],
      visitStage: ['Checkup', Validators.required],
      visitType: [null, Validators.required],
      scheduledStart: [null, Validators.required],
      reason: [null]
    });

    this.patientForm = this.fb.group({
      fullName: [null, Validators.required],
      phoneNumber: [null],
      email: [null, Validators.email]
    });

    this.bookingForm.get('specialty')?.valueChanges.subscribe((spec) => {
      this.bookingForm.get('doctorId')?.setValue(null);
      this.applyDoctorFilters(spec);
    });

    this.loadLookups();
  }

  private loadLookups(): void {
    this.isLoadingLookups.set(true);

    this.patientService.getPatients()
      .pipe(finalize(() => this.isLoadingLookups.set(false)))
      .subscribe({
        next: (data) => this.patients.set(data.items),
        error: () => this.serverError.set('تعذر تحميل قائمة المرضى')
      });

    this.doctorService.getDoctors().subscribe({
      next: (data) => {
        const scoped = data.items.filter((d) => d.isActive);
        this.allDoctors.set(scoped);
        this.applyDoctorFilters(this.bookingForm.get('specialty')?.value);

        const uniqueSpecialties = [
          ...new Set(
            scoped
              .map((d) => d.primarySpecialtyName)
              .filter((name): name is string => !!name)
          )
        ].map((name) => ({ label: name, value: name }));

        this.specialties.set(uniqueSpecialties);
      },
      error: () => this.serverError.set('تعذر تحميل قائمة الأطباء')
    });
  }

  private applyDoctorFilters(specialty: string | null): void {
    let list = this.allDoctors();
    if (specialty) {
      list = list.filter((d) => d.primarySpecialtyName === specialty);
    }
    this.filteredDoctors.set(list);
  }

  showNewPatient(): void {
    this.patientForm.reset();
    this.displayPatientDialog.set(true);
  }

  savePatient(): void {
    if (this.patientForm.invalid) {
      this.patientForm.markAllAsTouched();
      return;
    }

    this.isSavingPatient.set(true);
    this.patientService
      .createPatient(this.patientForm.value)
      .pipe(finalize(() => this.isSavingPatient.set(false)))
      .subscribe({
        next: (id) => {
          const fullName = this.patientForm.value.fullName as string;
          const phoneNumber = this.patientForm.value.phoneNumber as string | null;
          this.patients.update((list) => [
            ...list,
            {
              id,
              fullName,
              phoneNumber: phoneNumber ?? undefined,
              createdAt: new Date().toISOString()
            }
          ]);
          this.bookingForm.patchValue({ patientId: id });
          this.displayPatientDialog.set(false);
        },
        error: (err) => {
          if (err.error?.errors) {
            const firstErrorKey = Object.keys(err.error.errors)[0];
            this.serverError.set(err.error.errors[firstErrorKey][0]);
          } else {
            this.serverError.set('تعذر حفظ بيانات المريض');
          }
        }
      });
  }

  onSubmit(): void {
    if (this.bookingForm.invalid) {
      this.bookingForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.serverError.set('');

    const formVal = this.bookingForm.value;
    const start = new Date(formVal.scheduledStart);
    const end = new Date(start.getTime() + 30 * 60000);

    const command: CreateAppointmentCommand = {
      patientId: formVal.patientId,
      doctorId: formVal.doctorId,
      visitStage: formVal.visitStage,
      visitType: formVal.visitType,
      scheduledStart: start.toISOString(),
      scheduledEnd: end.toISOString(),
      reason: formVal.reason
    };

    this.appointmentService
      .createAppointment(command)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: () => this.router.navigate(['/appointments/search']),
        error: (err) => {
          if (err.error?.errors) {
            const firstErrorKey = Object.keys(err.error.errors)[0];
            this.serverError.set(err.error.errors[firstErrorKey][0]);
          } else {
            this.serverError.set('حدث خطأ أثناء حفظ الحجز. يرجى المحاولة مرة أخرى.');
          }
        }
      });
  }
}
