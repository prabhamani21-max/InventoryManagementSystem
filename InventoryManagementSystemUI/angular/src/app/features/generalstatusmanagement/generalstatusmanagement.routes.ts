// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { GeneralStatusmanagement } from './generalstatusmanagement';
import { GeneralStatustable } from './generalstatustable/generalstatustable';
import { GeneralStatusform } from './generalstatusform/generalstatusform';

/**
 * General Status Management Routes
 * Defines routing configuration for general status management feature
 */
export const GENERAL_STATUS_ROUTES: Routes = [
  {
    path: '',
    component: GeneralStatusmanagement,
    children: [
      {
        path: '',
        component: GeneralStatustable,
      },
      {
        path: 'add',
        component: GeneralStatusform,
      },
      {
        path: 'edit/:id',
        component: GeneralStatusform,
      },
    ],
  },
];