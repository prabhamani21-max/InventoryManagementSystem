// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Invoicepaymentmanagement } from './invoicepaymentmanagement';
import { Invoicepaymenttable } from './invoicepaymenttable/invoicepaymenttable';
import { Invoicepaymentform } from './invoicepaymentform/invoicepaymentform';

/**
 * InvoicePayment Management Routes
 * Defines routing configuration for payment management feature
 */
export const INVOICE_PAYMENT_ROUTES: Routes = [
  {
    path: '',
    component: Invoicepaymentmanagement,
    children: [
      {
        path: '',
        component: Invoicepaymenttable,
      },
      {
        path: 'add',
        component: Invoicepaymentform,
      },
      {
        path: 'edit/:id',
        component: Invoicepaymentform,
      },
    ],
  },
];
