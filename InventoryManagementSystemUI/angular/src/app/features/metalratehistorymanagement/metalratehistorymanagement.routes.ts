// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Metalratehistorymanagement } from './metalratehistorymanagement';
import { Metalratehistorytable } from './metalratehistorytable/metalratehistorytable';
import { Metalratehistoryform } from './metalratehistoryform/metalratehistoryform';

/**
 * Metal Rate History Management Routes
 * Defines routing configuration for metal rate history management feature
 */
export const METAL_RATE_HISTORY_ROUTES: Routes = [
  {
    path: '',
    component: Metalratehistorymanagement,
    children: [
      {
        path: '',
        component: Metalratehistorytable,
      },
      {
        path: 'add',
        component: Metalratehistoryform,
      },
      {
        path: 'edit/:id',
        component: Metalratehistoryform,
      },
    ],
  },
];
