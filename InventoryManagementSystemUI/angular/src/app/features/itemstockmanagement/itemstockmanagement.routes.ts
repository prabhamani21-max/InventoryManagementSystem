// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Itemstockmanagement } from './itemstockmanagement';
import { Itemstocktable } from './itemstocktable/itemstocktable';
import { Itemstockform } from './itemstockform/itemstockform';

/**
 * ItemStock Management Routes
 * Defines routing configuration for item stock management feature
 */
export const ITEM_STOCK_ROUTES: Routes = [
  {
    path: '',
    component: Itemstockmanagement,
    children: [
      {
        path: '',
        component: Itemstocktable,
      },
      {
        path: 'add',
        component: Itemstockform,
      },
      {
        path: 'edit/:id',
        component: Itemstockform,
      },
    ],
  },
];