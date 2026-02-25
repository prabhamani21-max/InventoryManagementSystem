// Angular Import
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// project import
import { AdminComponent } from './theme/layout/admin/admin.component';
import { AuthLayoutComponent } from './demo/pages/authentication/auth-layout.component';
import { authGuard } from './core/gaurds/auth.guard';

export const routes: Routes = [
   {
    path: '',
    redirectTo: 'jewelleryManagement/auth/sign-in',
    pathMatch: 'full',
  },
  {
    path: 'jewelleryManagement/auth',
    component: AuthLayoutComponent,
    loadChildren: () =>
      import('../app/demo/pages/authentication/auth.route').then((m) => m.AUTH_ROUTES),
  },
  {
    path: 'jewelleryManagement/admin',
    component: AdminComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'analytics',
        loadComponent: () => import('./demo/dashboard/dash-analytics.component').then((c) => c.DashAnalyticsComponent)
      },
      {
        path: 'category',
        loadChildren: () =>
          import('./features/categorymanagement/categorymanagement.routes').then((m) => m.CATEGORY_ROUTES),
      },
      {
        path: 'metal',
        loadChildren: () =>
          import('./features/metalmanagement/metalmanagement.routes').then((m) => m.METAL_ROUTES),
      },
      {
        path: 'purity',
        loadChildren: () =>
          import('./features/puritymanagement/puritymanagement.routes').then((m) => m.PURITY_ROUTES),
      }, 
      {
        path: 'stone',
        loadChildren: () =>
          import('./features/stonemanagement/stonemanagement.routes').then((m) => m.STONE_ROUTES),
      },
      {
        path: 'supplier',
        loadChildren: () =>
          import('./features/suppliermanagement/suppliermanagement.routes').then((m) => m.SUPPLIER_ROUTES),
      },
      
      {
        path: 'stoneratehistory',
       loadChildren: () => import('./features/stoneratehistorymanagement/stoneratehistorymanagement.routes').then(m => m.STONE_RATE_HISTORY_ROUTES)
      },
      
      {
        path: 'metalratehistory',
       loadChildren: () => import('./features/metalratehistorymanagement/metalratehistorymanagement.routes').then(m => m.METAL_RATE_HISTORY_ROUTES)
      },
       {
        path: 'jewellery',
       loadChildren: () => import('./features/jewelleryitemmanagement/jewelleryitemmanagement.routes').then(m => m.JEWELLERY_ITEM_ROUTES)
      },
      {
        path: 'itemstock',
        loadChildren: () => import('./features/itemstockmanagement/itemstockmanagement.routes').then(m => m.ITEM_STOCK_ROUTES)
      },
      {
        path: 'saleorder',
        loadChildren: () => import('./features/saleordermanagement/saleordermanagement.routes').then(m => m.SALE_ORDER_ROUTES)
      },
      {
        path: 'saleorderitem',
        loadChildren: () => import('./features/saleorderitemmanagement/saleorderitemmanagement.routes').then(m => m.SALE_ORDER_ITEM_ROUTES)
      },
      {
        path: 'invoice',
        loadChildren: () => import('./features/invoicemanagement/invoicemanagement.routes').then(m => m.INVOICE_ROUTES)
      },
      {
        path: 'invoiceitem',
        loadChildren: () => import('./features/invoiceitemmanagement/invoiceitemmanagement.routes').then(m => m.INVOICE_ITEM_ROUTES)
      },
      {
        path: 'payment',
        loadChildren: () => import('./features/paymentmanagement/paymentmanagement.routes').then(m => m.PAYMENT_ROUTES)
      },
      {
        path: 'tcs',
        loadChildren: () => import('./features/tcsmanagement/tcsmanagement.routes').then(m => m.TCS_ROUTES)
      },
      {
        path: 'exchange',
        loadChildren: () => import('./features/exchangemanagement/exchangemanagement.routes').then(m => m.EXCHANGE_ROUTES)
      },
      {
        path: 'user',
        loadChildren: () => import('./features/usermanagement/usermanagement.routes').then(m => m.USER_ROUTES)
      },
      {
        path: 'role',
        loadChildren: () => import('./features/rolemanagement/rolemanagement.routes').then(m => m.ROLE_ROUTES)
      },
      {
        path: 'warehouse',
        loadChildren: () => import('./features/warehousemanagement/warehousemanagement.routes').then(m => m.WAREHOUSE_ROUTES)
      },
      {
        path: 'sale-wizard',
        loadChildren: () => import('./features/salewizard/salewizard.routes').then(m => m.SALE_WIZARD_ROUTES)
      },
      {
        path: '',
        redirectTo: 'analytics',
        pathMatch: 'full'
      },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
