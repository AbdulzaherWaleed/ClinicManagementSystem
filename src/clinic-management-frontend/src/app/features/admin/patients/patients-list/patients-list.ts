import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PatientService } from '../../../../features/patients/services/patient.service';
import { PatientDto } from '../../../../features/patients/models/patient.models';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { SharedModule } from 'primeng/api';

import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-patients-list',
  standalone: true,
  imports: [CommonModule, CardModule, TableModule, ButtonModule, SharedModule, FormsModule, InputTextModule],
  templateUrl: './patients-list.html',
  styleUrls: ['./patients-list.scss']
})
export class PatientsListComponent implements OnInit {
  private patientService = inject(PatientService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  patients: PatientDto[] = [];
  isLoading = false;
  hasSearched = false;
  searchTerm = '';
  totalRecords = 0;

  ngOnInit() {
    // Requirements: Do not load any patient profile before a patient is selected.
    // Display only a search interface initially.
  }

  onSearch() {
    if (!this.searchTerm.trim()) {
       return;
    }
    this.hasSearched = true;
    this.loadPatients({ first: 0, rows: 10 });
  }

  loadPatients(event: any) {
    if (!this.hasSearched) return;
    
    this.isLoading = true;
    this.cdr.markForCheck();
    const pageNumber = (event.first / event.rows) + 1;
    const pageSize = event.rows;

    this.patientService.getPatients(pageNumber, pageSize, this.searchTerm).subscribe({
      next: (res) => {
        this.patients = res.items;
        this.totalRecords = res.totalCount;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('loadPatients error:', err);
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  goToProfile(patient: PatientDto) {
    this.router.navigate(['/patients', patient.id, 'profile']);
  }
}
