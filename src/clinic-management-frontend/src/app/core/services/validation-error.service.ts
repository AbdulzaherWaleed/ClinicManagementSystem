import { Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';

/**
 * ValidationErrorService — bridge between the ErrorInterceptor (global HTTP errors)
 * and individual Reactive Forms components.
 *
 * Usage in a form component:
 *   this.validationErrors.errors$.subscribe(errors => {
 *     Object.entries(errors).forEach(([field, messages]) => {
 *       const control = this.form.get(field.toLowerCase());
 *       control?.setErrors({ serverError: messages[0] });
 *     });
 *   });
 */
@Injectable({ providedIn: 'root' })
export class ValidationErrorService {
  private readonly _errors$ = new Subject<Record<string, string[]>>();

  /** Observable that form components subscribe to for server-side validation errors */
  readonly errors$ = this._errors$.asObservable();

  setErrors(errors: Record<string, string[]>): void {
    this._errors$.next(errors);
  }
}
