// Angular Import
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// project import
import { AdminComponent } from './theme/layout/admin/admin.component';
import { AuthLayoutComponent } from './demo/pages/authentication/auth-layout.component';
import { authGuard } from './core/gaurds/auth.guard';
import { RoleGuard } from './core/gaurds/role.guard';
import { RoleEnum } from './core/enums/role.enum';

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
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadComponent: () => import('./features/dashboard/dash-analytics.component').then((c) => c.DashAnalyticsComponent)
      },
      {
        path: 'customer',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.Customer] },
        loadComponent: () =>
          import('./features/customerdashboard/customerdashboard.component').then(
            (c) => c.CustomerDashboardComponent
          ),
        children: [
          {
            path: 'orders',
            loadComponent: () =>
              import('./features/customerdashboard/customer-orders/customer-orders.component').then(
                (c) => c.CustomerOrdersComponent
              )
          },
          {
            path: 'invoices',
            loadComponent: () =>
              import('./features/customerdashboard/customer-invoices/customer-invoices.component').then(
                (c) => c.CustomerInvoicesComponent
              )
          },
          {
            path: 'invoice/:invoiceNumber',
            loadComponent: () =>
              import('./features/invoicemanagement/invoiceform/invoiceform').then(
                (c) => c.Invoiceform
              )
          }
        ]
      },
      {
        path: 'sales-dashboard',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.Sales] },
        loadComponent: () =>
          import('./features/salesdashboard/salesdashboard.component').then(
            (c) => c.SalesDashboardComponent
          ),
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/salesdashboard/sales-overview/sales-overview.component').then(
                (c) => c.SalesOverviewComponent
              )
          },
          {
            path: 'orders',
            loadComponent: () =>
              import('./features/salesdashboard/sales-orders/sales-orders.component').then(
                (c) => c.SalesOrdersComponent
              )
          },
          {
            path: 'invoices',
            loadComponent: () =>
              import('./features/salesdashboard/sales-invoices/sales-invoices.component').then(
                (c) => c.SalesInvoicesComponent
              )
          },
          {
            path: 'customers',
            loadComponent: () =>
              import('./features/salesdashboard/sales-customers/sales-customers.component').then(
                (c) => c.SalesCustomersComponent
              )
          },
          {
            path: 'exchanges',
            loadComponent: () =>
              import('./features/salesdashboard/sales-exchanges/sales-exchanges.component').then(
                (c) => c.SalesExchangesComponent
              )
          }
        ]
      },
      {
        path: 'category',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () =>
          import('./features/categorymanagement/categorymanagement.routes').then((m) => m.CATEGORY_ROUTES),
      },
       {
         path: 'metal',
         canActivate: [RoleGuard],
         data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
         loadChildren: () =>
           import('./features/metalmanagement/metalmanagement.routes').then((m) => m.METAL_ROUTES),
       },
       {
         path: 'generalstatus',
         canActivate: [RoleGuard],
         data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
         loadChildren: () =>
           import('./features/generalstatusmanagement/generalstatusmanagement.routes').then((m) => m.GENERAL_STATUS_ROUTES),
       },
       {
         path: 'purity',
         canActivate: [RoleGuard],
         data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
         loadChildren: () =>
           import('./features/puritymanagement/puritymanagement.routes').then((m) => m.PURITY_ROUTES),
       }, 
      {
        path: 'stone',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () =>
          import('./features/stonemanagement/stonemanagement.routes').then((m) => m.STONE_ROUTES),
      },
      {
        path: 'supplier',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () =>
          import('./features/suppliermanagement/suppliermanagement.routes').then((m) => m.SUPPLIER_ROUTES),
      },
      
      {
        path: 'stoneratehistory',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
       loadChildren: () => import('./features/stoneratehistorymanagement/stoneratehistorymanagement.routes').then(m => m.STONE_RATE_HISTORY_ROUTES)
      },
      
      {
        path: 'metalratehistory',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
       loadChildren: () => import('./features/metalratehistorymanagement/metalratehistorymanagement.routes').then(m => m.METAL_RATE_HISTORY_ROUTES)
      },
       {
        path: 'jewellery',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
       loadChildren: () => import('./features/jewelleryitemmanagement/jewelleryitemmanagement.routes').then(m => m.JEWELLERY_ITEM_ROUTES)
      },
      {
        path: 'itemstock',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () => import('./features/itemstockmanagement/itemstockmanagement.routes').then(m => m.ITEM_STOCK_ROUTES)
      },
      {
        path: 'saleorder',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () => import('./features/saleordermanagement/saleordermanagement.routes').then(m => m.SALE_ORDER_ROUTES)
      },
      {
        path: 'saleorderitem',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () => import('./features/saleorderitemmanagement/saleorderitemmanagement.routes').then(m => m.SALE_ORDER_ITEM_ROUTES)
      },
      {
        path: 'invoice',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () => import('./features/invoicemanagement/invoicemanagement.routes').then(m => m.INVOICE_ROUTES)
      },
      {
        path: 'invoiceitem',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () => import('./features/invoiceitemmanagement/invoiceitemmanagement.routes').then(m => m.INVOICE_ITEM_ROUTES)
      },
      {
        path: 'payment',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () => import('./features/paymentmanagement/paymentmanagement.routes').then(m => m.PAYMENT_ROUTES)
      },
      {
        path: 'tcs',
        loadChildren: () => import('./features/tcsmanagement/tcsmanagement.routes').then(m => m.TCS_ROUTES)
      },
      {
        path: 'exchange',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager, RoleEnum.Sales] },
        loadChildren: () => import('./features/exchangemanagement/exchangemanagement.routes').then(m => m.EXCHANGE_ROUTES)
      },
      {
        path: 'user',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager, RoleEnum.Sales] },
        loadChildren: () => import('./features/usermanagement/usermanagement.routes').then(m => m.USER_ROUTES)
      },
      {
        path: 'role',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin] },
        loadChildren: () => import('./features/rolemanagement/rolemanagement.routes').then(m => m.ROLE_ROUTES)
      },
      {
        path: 'warehouse',
        canActivate: [RoleGuard],
        data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager] },
        loadChildren: () => import('./features/warehousemanagement/warehousemanagement.routes').then(m => m.WAREHOUSE_ROUTES)
      },
      {
        path: 'sale-wizard',
        loadChildren: () => import('./features/salewizard/salewizard.routes').then(m => m.SALE_WIZARD_ROUTES)
      },
      {
  path: 'userkyc',
  canActivate: [RoleGuard],
  data: { roleId: [RoleEnum.SuperAdmin, RoleEnum.Manager, RoleEnum.Sales] },
  loadChildren: () => import('./features/userkycmanagement/userkycmanagement.routes').then(m => m.USER_KYC_ROUTES)
},
      {
        path: '',
        redirectTo: 'analytics',
        pathMatch: 'full'
      },
    ]
  },
  {
    path: 'jewelleryManagement/unauthorized',
    loadComponent: () =>
      import('./common/unauthorized/unauthorized.component').then((c) => c.Unauthorized),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
