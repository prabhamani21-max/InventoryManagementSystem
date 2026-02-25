import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthenticationService } from '../services/auth.service';   
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthenticationService);
  const router = inject(Router);
  const toastr = inject(ToastrService, { optional: true });


 // Check if token exists and is not expired
 if (authService.isAuthenticated() && !authService.isTokenExpired()) {
  return true;
}

// Token is either missing or expired
if (authService.isTokenExpired()) {
  toastr?.warning('Your session has expired. Please log in again.');
} else {
  toastr?.info('Please log in to access this page');
}

// Clear any existing session data
authService.logout();
  
  return router.parseUrl('/jewelleryManagement/auth/sign-in');
};