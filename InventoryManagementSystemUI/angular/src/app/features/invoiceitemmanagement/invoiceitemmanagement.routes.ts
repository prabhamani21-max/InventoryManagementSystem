// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Invoiceitemmanagement } from './invoiceitemmanagement';
import { Invoiceitemtable } from './invoiceitemtable/invoiceitemtable';
import { Invoiceitemform } from './invoiceitemform/invoiceitemform';

/**
 * InvoiceItem Management Routes
 * Defines routing configuration for invoice item management feature
 */
export const INVOICE_ITEM_ROUTES: Routes = [
  {
    path: '',
    component: Invoiceitemmanagement,
    children: [
      {
        path: '',
        component: Invoiceitemtable,
      },
      {
        path: 'add',
        component: Invoiceitemform,
      },
      {
        path: 'edit/:id',
        component: Invoiceitemform,
      },
    ],
  },
];
