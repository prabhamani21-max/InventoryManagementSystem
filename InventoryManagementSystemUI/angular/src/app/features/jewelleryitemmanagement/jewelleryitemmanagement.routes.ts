// Angular Import
import { Routes } from '@angular/router';

// Project Import
import { Jewelleryitemmanagement } from './jewelleryitemmanagement';
import { Jewelleryitemtable } from './jewelleryitemtable/jewelleryitemtable';
import { Jewelleryitemform } from './jewelleryitemform/jewelleryitemform';

/**
 * JewelleryItem Management Routes
 * Defines routing configuration for jewellery item management feature
 */
export const JEWELLERY_ITEM_ROUTES: Routes = [
  {
    path: '',
    component: Jewelleryitemmanagement,
    children: [
      {
        path: '',
        component: Jewelleryitemtable,
      },
      {
        path: 'add',
        component: Jewelleryitemform,
      },
      {
        path: 'edit/:id',
        component: Jewelleryitemform,
      },
    ],
  },
];
