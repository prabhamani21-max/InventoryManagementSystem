// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Metalmanagement } from './metalmanagement';
import { Metaltable } from './metaltable/metaltable';
import { Metalform } from './metalform/metalform';

/**
 * Metal Management Routes
 * Defines routing configuration for metal management feature
 */
export const METAL_ROUTES: Routes = [
  {
    path: '',
    component: Metalmanagement,
    children: [
      {
        path: '',
        component: Metaltable,
      },
      {
        path: 'add',
        component: Metalform,
      },
      {
        path: 'edit/:id',
        component: Metalform,
      },
    ],
  },
];
