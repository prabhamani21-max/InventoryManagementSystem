// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Tcsmanagement } from './tcsmanagement';
import { Tcsreport } from './tcsreport/tcsreport';

/**
 * TCS Management Routes
 * Defines routing configuration for TCS (Tax Collected at Source) management feature
 */
export const TCS_ROUTES: Routes = [
  {
    path: '',
    component: Tcsmanagement,
    children: [
      {
        path: '',
        component: Tcsreport,
      },
      {
        path: 'report',
        component: Tcsreport,
      },
    ],
  },
];
