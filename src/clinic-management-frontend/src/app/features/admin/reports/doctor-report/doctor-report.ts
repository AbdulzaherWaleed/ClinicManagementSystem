import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService, PerformanceReportDto } from '../services/report.service';
import { DoctorService } from '../../../admin/doctors/services/doctor.service';

import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-doctor-report',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, CardModule, ChartModule, 
    SelectModule, DatePickerModule, ButtonModule, ToastModule
  ],
  providers: [MessageService],
  templateUrl: './doctor-report.html',
  styleUrls: ['./doctor-report.scss']
})
export class DoctorReport implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly reportService = inject(ReportService);
  private readonly doctorService = inject(DoctorService);
  private readonly messageService = inject(MessageService);

  filterForm!: FormGroup;
  doctors = signal<any[]>([]);
  reportData = signal<PerformanceReportDto | null>(null);
  isLoading = signal<boolean>(false);
  
  chartData: any;
  chartOptions: any;

  ngOnInit() {
    this.initForm();
    this.loadDoctors();
    this.initChartOptions();
  }

  initForm() {
    const today = new Date();
    const thirtyDaysAgo = new Date();
    thirtyDaysAgo.setDate(today.getDate() - 30);

    this.filterForm = this.fb.group({
      doctorId: [null, Validators.required],
      startDate: [thirtyDaysAgo, Validators.required],
      endDate: [today, Validators.required]
    });
  }

  loadDoctors() {
    this.doctorService.getDoctors().subscribe({
      next: (data: any) => {
        const list = data.items || data;
        this.doctors.set(list.map((d: any) => ({ label: d.fullName, value: d.id })));
      }
    });
  }

  initChartOptions() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');

    this.chartOptions = {
      plugins: {
        legend: { labels: { color: textColor } }
      },
      responsive: true
    };
  }

  generateReport() {
    if (this.filterForm.invalid) {
      this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: 'يرجى ملء جميع الحقول المطلوبة' });
      return;
    }

    this.isLoading.set(true);
    const val = this.filterForm.value;

    this.reportService.getDoctorPerformance(val.doctorId, val.startDate, val.endDate)
      .subscribe({
        next: (data) => {
          this.reportData.set(data);
          this.setupChartData(data);
          this.isLoading.set(false);
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء تحميل التقرير' });
          this.isLoading.set(false);
        }
      });
  }

  setupChartData(data: PerformanceReportDto) {
    const documentStyle = getComputedStyle(document.documentElement);
    
    const stageTranslations: { [key: string]: string } = {
      'Checkup': 'كشف',
      'Consultation': 'استشارة',
      'Procedure': 'إجراء / جراحة'
    };

    const labels = Object.keys(data.bookingsByVisitStage).map(k => stageTranslations[k] || k);
    const values = Object.values(data.bookingsByVisitStage);

    this.chartData = {
      labels: labels,
      datasets: [
        {
          data: values,
          backgroundColor: [
            documentStyle.getPropertyValue('--p-blue-500'),
            documentStyle.getPropertyValue('--p-orange-500'),
            documentStyle.getPropertyValue('--p-green-500')
          ],
          hoverBackgroundColor: [
            documentStyle.getPropertyValue('--p-blue-400'),
            documentStyle.getPropertyValue('--p-orange-400'),
            documentStyle.getPropertyValue('--p-green-400')
          ]
        }
      ]
    };
  }
}
