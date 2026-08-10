import { Directive, Input, TemplateRef, ViewContainerRef, effect } from '@angular/core';
import { PermissionService } from '../../core/auth/services/permission.service';
import { AuthService } from '../../core/auth/services/auth.service';

@Directive({
  selector: '[appHasPermission]',
  standalone: true
})
export class HasPermissionDirective {
  private permission: string = '';
  private hasView = false;

  constructor(
    private readonly templateRef: TemplateRef<any>,
    private readonly viewContainer: ViewContainerRef,
    private readonly permissionService: PermissionService,
    private readonly authService: AuthService
  ) {
    // Re-evaluate whenever current user changes
    effect(() => {
      this.authService.currentUser();
      this.updateView();
    });
  }

  @Input() set appHasPermission(permission: string) {
    this.permission = permission;
    this.updateView();
  }

  private updateView() {
    if (!this.permission) return;

    const hasPermission = this.permissionService.hasPermission(this.permission);

    if (hasPermission && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasPermission && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
