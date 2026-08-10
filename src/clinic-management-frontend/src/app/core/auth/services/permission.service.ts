import { Injectable, computed } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  constructor(private readonly authService: AuthService) {}

  /**
   * Parses the JWT token to extract claims.
   */
  private getPermissions(): string[] {
    const token = this.authService.getToken();
    if (!token) return [];

    try {
      const payload = token.split('.')[1];
      // Convert Base64Url to Base64
      let base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      
      // Pad with '='
      const pad = base64.length % 4;
      if (pad) {
        base64 += new Array(5 - pad).join('=');
      }

      const decoded = atob(base64);
      // Decode UTF-8 properly to avoid garbling Arabic names (which can sometimes break JSON.parse)
      const utf8Decoded = decodeURIComponent(
        decoded.split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join('')
      );
      
      const parsed = JSON.parse(utf8Decoded);
      
      // JWT may store multiple claims of the same name as an array or a single string
      const permissions = parsed['Permission'];
      if (!permissions) return [];

      return Array.isArray(permissions) ? permissions : [permissions];
    } catch {
      return [];
    }
  }

  /**
   * Checks if the user has a specific permission.
   * Admin always returns true.
   */
  hasPermission(permission: string): boolean {
    if (this.authService.isAdmin()) return true;
    
    // For now, if the role isn't Admin, we verify the specific permission
    const permissions = this.getPermissions();
    return permissions.includes(permission);
  }
}

