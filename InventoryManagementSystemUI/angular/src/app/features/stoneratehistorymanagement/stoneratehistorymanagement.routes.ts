// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Stoneratehistorymanagement } from './stoneratehistorymanagement';
import { Stoneratehistorytable } from './stoneratehistorytable/stoneratehistorytable';
import { Stoneratehistoryform } from './stoneratehistoryform/stoneratehistoryform';

/**
 * Stone Rate History Management Routes
 * Defines routing configuration for stone rate history management feature
 */
export const STONE_RATE_HISTORY_ROUTES: Routes = [
  {
    path: '',
    component: Stoneratehistorymanagement,
    children: [
      {
        path: '',
        component: Stoneratehistorytable,
      },
      {
        path: 'add',
        component: Stoneratehistoryform,
      },
      {
        path: 'edit/:id',
        component: Stoneratehistoryform,
      },
    ],
  },
];
