import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../core/auth/services/auth.service';
import { ValidationErrorService } from '../../../core/services/validation-error.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  private readonly fb               = inject(FormBuilder);
  private readonly authService      = inject(AuthService);
  private readonly router           = inject(Router);
  private readonly route            = inject(ActivatedRoute);
  private readonly validationErrors = inject(ValidationErrorService);

  loginForm!: FormGroup;
  showPassword  = false;
  isLoading     = false;
  serverError   = '';
  readonly currentYear = new Date().getFullYear();
  private returnUrl = '/dashboard';
  private subs = new Subscription();

  readonly features = [
    { icon: 'pi pi-calendar', title: 'إدارة الحجوزات', desc: 'جدولة ومتابعة المواعيد بسهولة' },
    { icon: 'pi pi-users',    title: 'ملفات المرضى',   desc: 'سجلات طبية شاملة ومنظمة' },
    { icon: 'pi pi-chart-bar', title: 'التقارير والتحليلات', desc: 'تقارير أداء تفصيلية للأطباء' }
  ];

  ngOnInit(): void {
    // Redirect if already logged in
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
      return;
    }

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] ?? '/dashboard';

    this.loginForm = this.fb.group({
      email:    ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });

    // Wire FluentValidation server errors from interceptor
    this.subs.add(
      this.validationErrors.errors$.subscribe(errors => {
        this.applyServerErrors(errors);
      })
    );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.serverError = '';

    const { email, password } = this.loginForm.value;

    this.authService.login({ email, password }).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (err) => {
        this.isLoading = false;
        // Generic error if interceptor didn't handle it via validationErrors$
        if (err.status === 400 && !err.error?.errors) {
          this.serverError = 'البريد الإلكتروني أو كلمة المرور غير صحيحة';
        } else if (err.status === 0) {
          this.serverError = 'تعذر الاتصال بالخادم، يرجى التحقق من الاتصال بالإنترنت';
        }
      }
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  // Getters for template cleanliness
  get emailControl()    { return this.loginForm.get('email')!; }
  get passwordControl() { return this.loginForm.get('password')!; }

  get emailError(): string {
    const c = this.emailControl;
    if (c.hasError('serverError')) return c.errors?.['serverError'];
    if (c.hasError('required'))    return 'البريد الإلكتروني مطلوب';
    if (c.hasError('email'))       return 'تنسيق البريد الإلكتروني غير صحيح';
    return '';
  }

  get passwordError(): string {
    const c = this.passwordControl;
    if (c.hasError('serverError')) return c.errors?.['serverError'];
    if (c.hasError('required'))    return 'كلمة المرور مطلوبة';
    if (c.hasError('minlength'))   return 'كلمة المرور يجب أن تكون 8 أحرف على الأقل';
    return '';
  }

  private applyServerErrors(errors: Record<string, string[]>): void {
    Object.entries(errors).forEach(([field, messages]) => {
      const control = this.loginForm.get(field.toLowerCase());
      if (control) {
        control.setErrors({ serverError: messages[0] });
      } else {
        this.serverError = messages[0] ?? 'حدث خطأ غير متوقع';
      }
    });
  }
}
