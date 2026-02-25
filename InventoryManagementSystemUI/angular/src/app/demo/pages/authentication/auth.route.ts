import { Route } from '@angular/router';
import { SignInComponent } from './sign-in/sign-in.component';
import { noAuthGuard } from '../../../core/gaurds/noauth.guard ';
import { SignUpComponent } from './sign-up/sign-up.component';

export const AUTH_ROUTES: Route[] = [
  {
    path: 'sign-in',
    component: SignInComponent,
    canActivate: [noAuthGuard],
    data: { title: 'Sign In' },
  },
  {
    path: 'sign-up',
    component: SignUpComponent,
    canActivate: [noAuthGuard],
    data: { title: 'Sign Up' },
  },

  {
    path: '',
    redirectTo: 'sign-in',
    pathMatch: 'full',
  },
];
