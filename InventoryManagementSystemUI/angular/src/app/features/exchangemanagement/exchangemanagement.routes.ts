// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Exchangemanagement } from './exchangemanagement';
import { Exchangetable } from './exchangetable/exchangetable';
import { Exchangeform } from './exchangeform/exchangeform';

/**
 * Exchange Management Routes
 * Defines routing configuration for exchange management feature
 */
export const EXCHANGE_ROUTES: Routes = [
  {
    path: '',
    component: Exchangemanagement,
    children: [
      {
        path: '',
        component: Exchangetable,
      },
      {
        path: 'add',
        component: Exchangeform,
      },
      {
        path: 'edit/:id',
        component: Exchangeform,
      },
      {
        path: 'view/:id',
        component: Exchangeform,
      },
    ],
  },
];