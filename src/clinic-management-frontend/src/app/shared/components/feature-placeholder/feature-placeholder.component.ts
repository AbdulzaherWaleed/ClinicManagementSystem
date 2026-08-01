import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-feature-placeholder',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-container animate-fade-in">
      <div class="card p-6 max-w-2xl mx-auto text-center">
        <i class="pi pi-wrench text-4xl text-primary mb-4"></i>
        <h1 class="text-2xl font-bold mb-2">{{ title }}</h1>
        <p class="text-color-secondary">{{ description }}</p>
      </div>
    </div>
  `
})
export class FeaturePlaceholderComponent {
  private readonly route = inject(ActivatedRoute);

  readonly title = this.route.snapshot.data['title'] as string ?? 'قريباً';
  readonly description =
    this.route.snapshot.data['description'] as string ??
    'هذا القسم قيد التطوير وسيتوفر في تحديث لاحق.';
}
