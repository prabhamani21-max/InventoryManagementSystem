// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Categorymanagement } from './categorymanagement';
import { Categorytable } from './categorytable/categorytable';
import { Categoryform } from './categoryform/categoryform';

/**
 * Category Management Routes
 * Defines routing configuration for category management feature
 */
export const CATEGORY_ROUTES: Routes = [
  {
    path: '',
    component: Categorymanagement,
    children: [
      {
        path: '',
        component: Categorytable,
      },
      {
        path: 'add',
        component: Categoryform,
      },
      {
        path: 'edit/:id',
        component: Categoryform,
      },
    ],
  },
];