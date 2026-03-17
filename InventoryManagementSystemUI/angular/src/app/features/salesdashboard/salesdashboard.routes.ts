import { Routes } from '@angular/router';
import { RoleEnum } from 'src/app/core/enums/role.enum';

export const SALES_DASHBOARD_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./sales-overview/sales-overview.component').then(
        (c) => c.SalesOverviewComponent
      )
  },
  {
    path: 'orders',
    loadComponent: () =>
      import('./sales-orders/sales-orders.component').then(
        (c) => c.SalesOrdersComponent
      )
  },
  {
    path: 'invoices',
    loadComponent: () =>
      import('./sales-invoices/sales-invoices.component').then(
        (c) => c.SalesInvoicesComponent
      )
  },
  {
    path: 'customers',
    loadComponent: () =>
      import('./sales-customers/sales-customers.component').then(
        (c) => c.SalesCustomersComponent
      )
  },
  {
    path: 'exchanges',
    loadComponent: () =>
      import('./sales-exchanges/sales-exchanges.component').then(
        (c) => c.SalesExchangesComponent
      )
  }
];
