// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Paymentmanagement } from './paymentmanagement';
import { Paymenttable } from './paymenttable/paymenttable';
import { Paymentform } from './paymentform/paymentform';

/**
 * Payment Management Routes
 * Defines routing configuration for payment management feature
 */
export const PAYMENT_ROUTES: Routes = [
  {
    path: '',
    component: Paymentmanagement,
    children: [
      {
        path: '',
        component: Paymenttable,
      },
      {
        path: 'add',
        component: Paymentform,
      },
      {
        path: 'edit/:id',
        component: Paymentform,
      },
    ],
  },
];
