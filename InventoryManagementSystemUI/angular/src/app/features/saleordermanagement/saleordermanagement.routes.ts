// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Saleordermanagement } from './saleordermanagement';
import { Saleordertable } from './saleordertable/saleordertable';
import { Saleorderform } from './saleorderform/saleorderform';

/**
 * SaleOrder Management Routes
 * Defines routing configuration for sale order management feature
 */
export const SALE_ORDER_ROUTES: Routes = [
  {
    path: '',
    component: Saleordermanagement,
    children: [
      {
        path: '',
        component: Saleordertable,
      },
      {
        path: 'add',
        component: Saleorderform,
      },
      {
        path: 'edit/:id',
        component: Saleorderform,
      },
    ],
  },
];
