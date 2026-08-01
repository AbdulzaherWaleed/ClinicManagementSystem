import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule, SidebarComponent, TopbarComponent],
  template: `
    <div class="app-shell">
      <app-sidebar></app-sidebar>
      <div class="main-content">
        <app-topbar [pageTitle]="pageTitle"></app-topbar>
        <main class="content-area">
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>
  `,
  styles: [`
    @use '../../../../styles/variables' as *;

    .app-shell {
      display: flex;
      height: 100vh;
      overflow: hidden;
      direction: rtl;
    }

    .main-content {
      flex: 1;
      margin-right: $sidebar-width;
      display: flex;
      flex-direction: column;
      overflow: hidden;
      transition: margin-right 250ms ease;
    }

    .content-area {
      flex: 1;
      margin-top: 64px;
      overflow-y: auto;
      background: #f8f7ff;
    }
  `]
})
export class MainLayoutComponent {
  pageTitle = 'لوحة التحكم';
}
