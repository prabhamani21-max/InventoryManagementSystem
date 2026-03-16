// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Usermanagement } from './usermanagement';
import { Usertable } from './usertable/usertable';
import { Userform } from './userform/userform';

/**
 * User Management Routes
 * Defines routing configuration for user management feature
 */
export const USER_ROUTES: Routes = [
  {
    path: '',
    component: Usermanagement,
    children: [
      {
        path: '',
        component: Usertable,
      },
      {
        path: 'add',
        component: Userform,
      },
      {
        path: 'edit/:id',
        component: Userform,
      },
      {
        path: 'view/:id',
        component: Userform,
      },
    ],
  },
];