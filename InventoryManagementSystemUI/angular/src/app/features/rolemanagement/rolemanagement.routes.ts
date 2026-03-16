// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Rolemanagement } from './rolemanagement';
import { Roletable } from './roletable/roletable';
import { Roleform } from './roleform/roleform';

/**
 * Role Management Routes
 * Defines routing configuration for role management feature
 */
export const ROLE_ROUTES: Routes = [
  {
    path: '',
    component: Rolemanagement,
    children: [
      {
        path: '',
        component: Roletable,
      },
      {
        path: 'add',
        component: Roleform,
      },
      {
        path: 'edit/:id',
        component: Roleform,
      },
      {
        path: 'view/:id',
        component: Roleform,
      },
    ],
  },
];
