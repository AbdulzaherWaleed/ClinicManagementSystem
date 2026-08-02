import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div style="height:100vh;display:flex;flex-direction:column;align-items:center;
                justify-content:center;gap:16px;direction:rtl;font-family:'Cairo',sans-serif">
      <div style="font-size:64px">🚫</div>
      <h1 style="font-size:24px;font-weight:700;color:#1e1b4b">غير مصرح بالوصول</h1>
      <p style="color:#6b7280;font-size:15px">ليس لديك صلاحية للوصول إلى هذه الصفحة</p>
      <a routerLink="/"
         style="background:linear-gradient(135deg,#7c3aed,#6d28d9);color:white;
                padding:10px 24px;border-radius:10px;font-weight:600;font-size:14px">
        العودة للرئيسية
      </a>
    </div>
  `
})
export class UnauthorizedComponent {}
