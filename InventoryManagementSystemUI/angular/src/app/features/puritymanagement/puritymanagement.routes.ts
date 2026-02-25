// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Puritymanagement } from './puritymanagement';
import { Puritytable } from './puritytable/puritytable';
import { Purityform } from './purityform/purityform';

/**
 * Purity Management Routes
 * Defines routing configuration for purity management feature
 */
export const PURITY_ROUTES: Routes = [
  {
    path: '',
    component: Puritymanagement,
    children: [
      {
        path: '',
        component: Puritytable,
      },
      {
        path: 'add',
        component: Purityform,
      },
      {
        path: 'edit/:id',
        component: Purityform,
      },
    ],
  },
];
