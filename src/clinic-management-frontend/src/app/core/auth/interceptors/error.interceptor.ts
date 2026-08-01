import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ValidationErrorService } from '../../services/validation-error.service';

/**
 * Error Interceptor — handles API errors globally:
 * - 401 → clear token, redirect to /auth/login
 * - 403 → show toast "ليس لديك صلاحية"
 * - 400 → parse FluentValidation errors {"errors": {"FieldName": ["msg"]}}
 * - 500 → show generic error toast
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router           = inject(Router);
  const authService      = inject(AuthService);
  const validationErrors = inject(ValidationErrorService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case 401:
          authService.logout();
          break;

        case 403:
          // Toast notification (will be wired to PrimeNG MessageService)
          console.error('403 Forbidden — ليس لديك صلاحية للوصول لهذا المورد');
          break;

        case 400:
          // FluentValidation shape: { "errors": { "FieldName": ["Error message"] } }
          if (error.error?.errors) {
            validationErrors.setErrors(error.error.errors as Record<string, string[]>);
          }
          break;

        case 422:
          if (error.error?.errors) {
            validationErrors.setErrors(error.error.errors as Record<string, string[]>);
          }
          break;

        case 500:
        case 503:
          console.error('Server error — حدث خطأ في الخادم، يرجى المحاولة لاحقاً');
          break;
      }

      return throwError(() => error);
    })
  );
};
