// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Saleorderitemmanagement } from './saleorderitemmanagement';
import { Saleorderitemtable } from './saleorderitemtable/saleorderitemtable';
import { Saleorderitemform } from './saleorderitemform/saleorderitemform';

/**
 * SaleOrderItem Management Routes
 * Defines routing configuration for sale order item management feature
 */
export const SALE_ORDER_ITEM_ROUTES: Routes = [
  {
    path: '',
    component: Saleorderitemmanagement,
    children: [
      {
        path: '',
        component: Saleorderitemtable,
      },
      {
        path: 'add',
        component: Saleorderitemform,
      },
      {
        path: 'edit/:id',
        component: Saleorderitemform,
      },
    ],
  },
];
