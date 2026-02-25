import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router,
} from '@angular/router';
import { AuthenticationService } from '../services/auth.service'; // Adjust path as needed

@Injectable({
  providedIn: 'root',
})
export class RoleGuard implements CanActivate {
  constructor(
    private router: Router,
    private authService: AuthenticationService,
  ) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
  ): boolean | UrlTree {
    const expectedRoleIds: number[] = next.data['roleId'];
    const decodedToken = this.authService.getUserInformation();

    if (!decodedToken || !decodedToken.roleId) {
      console.warn('No valid token or roleId found');
      return this.router.parseUrl('/jewelleryManagement/unauthorized');
    }

    const userRoleId = parseInt(decodedToken.roleId, 10);
    console.debug(
      `Checking access: userRoleId=${userRoleId}, expectedRoleIds=${expectedRoleIds}`,
    ); // Debug log
    if (isNaN(userRoleId)) {
      console.warn('Invalid roleId format in token');
      return this.router.parseUrl('/jewelleryManagement/unauthorized');
    }

    if (!expectedRoleIds || expectedRoleIds.includes(userRoleId)) {
      return true;
    }

    console.warn(
      `User roleId "${userRoleId}" not in expected roleIds: ${expectedRoleIds}`,
    );
    return this.router.parseUrl('/jewelleryManagement/unauthorized');
  }
}
