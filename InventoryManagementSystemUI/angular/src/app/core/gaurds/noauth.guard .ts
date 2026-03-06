import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthenticationService } from '../services/auth.service';
import { Router } from '@angular/router';
import { RoleEnum } from '../enums/role.enum';

export const noAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthenticationService);
  const router = inject(Router);

  if (!authService.isAuthenticated() || authService.isTokenExpired()) {
    return true;
  }

  const currentUser = authService.getUserInformation();
  const roleId = currentUser?.roleId;

  switch (roleId) {
    case RoleEnum.Customer.toString():
      return router.parseUrl('/jewelleryManagement/admin/customer');
    case RoleEnum.Sales.toString():
      return router.parseUrl('/jewelleryManagement/admin/exchange');
    default:
      return router.parseUrl('/jewelleryManagement/admin/analytics');
  }
};
