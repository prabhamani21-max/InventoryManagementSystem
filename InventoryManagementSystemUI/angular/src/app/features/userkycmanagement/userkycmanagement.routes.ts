// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { UserKycManagement } from './userkycmanagement';
import { UserKyctable } from './userkyctable/userkyctable';
import { UserKycform } from './userkycform/userkycform';

/**
 * UserKyc Management Routes
 * Defines routing configuration for UserKyc management feature
 */
export const USER_KYC_ROUTES: Routes = [
  {
    path: '',
    component: UserKycManagement,
    children: [
      {
        path: '',
        component: UserKyctable,
      },
      {
        path: 'add',
        component: UserKycform,
      },
      {
        path: 'edit/:id',
        component: UserKycform,
      },
      {
        path: 'view/:id',
        component: UserKycform,
      },
    ],
  },
];
