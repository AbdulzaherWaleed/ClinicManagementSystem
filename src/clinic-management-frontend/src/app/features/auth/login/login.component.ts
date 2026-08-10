import { Component, inject, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription, finalize } from 'rxjs';
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
  private readonly cdr              = inject(ChangeDetectorRef);

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
      const role = this.authService.userRole();
      this.router.navigate([role === 'Admin' ? '/dashboard' : '/appointments/search']);
      return;
    }

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] ?? '/dashboard';

    this.loginForm = this.fb.group({
      email:    ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });

    // We handle errors directly in onSubmit to use specific Arabic messages
    // instead of relying on the global ValidationErrorService for this component.
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

    this.authService.login({ email, password }).pipe(
      finalize(() => {
        this.isLoading = false;
        this.cdr.markForCheck();
      })
    ).subscribe({
      next: () => {
        let finalUrl = this.returnUrl;
        if (finalUrl === '/dashboard' && this.authService.userRole() !== 'Admin') {
          finalUrl = '/';
        }
        this.router.navigateByUrl(finalUrl);
      },
      error: (err) => {
        if (err.status === 429) {
          this.serverError = 'عدد كبير من المحاولات، برجاء الانتظار قليلاً';
        } else if (err.status === 400 && err.error?.errors) {
          const emailErrors = err.error.errors['Email'] || [];
          const passwordErrors = err.error.errors['Password'] || [];
          const hasLockout = emailErrors.some((e: string) => e.includes('locked'));
          
          if (hasLockout) {
            this.serverError = 'تم قفل الحساب مؤقتاً بسبب محاولات دخول خاطئة متكررة، حاول مرة أخرى بعد 5 دقائق';
          } else {
            this.serverError = 'البريد الإلكتروني أو كلمة المرور غير صحيحة';
          }
        } else if (err.status === 0) {
          this.serverError = 'تعذر الاتصال بالخادم، يرجى التحقق من الاتصال بالإنترنت';
        } else {
          this.serverError = 'البريد الإلكتروني أو كلمة المرور غير صحيحة';
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

}
