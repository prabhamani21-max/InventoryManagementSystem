// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Suppliermanagement } from './suppliermanagement';
import { Suppliertable } from './suppliertable/suppliertable';
import { Supplierform } from './supplierform/supplierform';

/**
 * Supplier Management Routes
 * Defines routing configuration for supplier management feature
 */
export const SUPPLIER_ROUTES: Routes = [
  {
    path: '',
    component: Suppliermanagement,
    children: [
      {
        path: '',
        component: Suppliertable,
      },
      {
        path: 'add',
        component: Supplierform,
      },
      {
        path: 'edit/:id',
        component: Supplierform,
      },
    ],
  },
];
