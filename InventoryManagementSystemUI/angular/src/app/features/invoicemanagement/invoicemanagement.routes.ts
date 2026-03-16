// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Invoicemanagement } from './invoicemanagement';
import { Invoicetable } from './invoicetable/invoicetable';
import { Invoiceform } from './invoiceform/invoiceform';

/**
 * Invoice Management Routes
 * Defines routing configuration for invoice management feature
 */
export const INVOICE_ROUTES: Routes = [
  {
    path: '',
    component: Invoicemanagement,
    children: [
      {
        path: '',
        component: Invoicetable,
      },
      {
        path: 'generate',
        component: Invoiceform,
      },
      {
        path: 'view/:invoiceNumber',
        component: Invoiceform,
      },
    ],
  },
];