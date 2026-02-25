// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Stonemanagement } from './stonemanagement';
import { Stonetable } from './stonetable/stonetable';
import { Stoneform } from './stoneform/stoneform';

/**
 * Stone Management Routes
 * Defines routing configuration for stone management feature
 */
export const STONE_ROUTES: Routes = [
  {
    path: '',
    component: Stonemanagement,
    children: [
      {
        path: '',
        component: Stonetable,
      },
      {
        path: 'add',
        component: Stoneform,
      },
      {
        path: 'edit/:id',
        component: Stoneform,
      },
    ],
  },
];
