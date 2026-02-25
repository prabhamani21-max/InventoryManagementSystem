// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Warehousemanagement } from './warehousemanagement';
import { Warehousetable } from './warehousetable/warehousetable';
import { Warehouseform } from './warehouseform/warehouseform';

/**
 * Warehouse Management Routes
 * Defines routing configuration for warehouse management feature
 */
export const WAREHOUSE_ROUTES: Routes = [
  {
    path: '',
    component: Warehousemanagement,
    children: [
      {
        path: '',
        component: Warehousetable,
      },
      {
        path: 'add',
        component: Warehouseform,
      },
      {
        path: 'edit/:id',
        component: Warehouseform,
      },
      {
        path: 'view/:id',
        component: Warehouseform,
      },
    ],
  },
];
