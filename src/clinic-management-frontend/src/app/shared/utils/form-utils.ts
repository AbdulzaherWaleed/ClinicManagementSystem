import { FormGroup } from '@angular/forms';

/**
 * Returns only the values of controls that have been dirtied (changed) by the user.
 * Use this to build PATCH payloads — only sends what was actually modified.
 *
 * @example
 * const patch = getDirtyValues(this.editForm);
 * this.service.update(id, patch).subscribe(...);
 */
export function getDirtyValues(form: FormGroup): Record<string, unknown> {
  const dirty: Record<string, unknown> = {};
  Object.keys(form.controls).forEach(key => {
    if (form.controls[key].dirty) {
      dirty[key] = form.controls[key].value;
    }
  });
  return dirty;
}
