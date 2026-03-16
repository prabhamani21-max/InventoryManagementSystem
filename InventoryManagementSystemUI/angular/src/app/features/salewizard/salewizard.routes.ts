import { Routes } from '@angular/router';
import { SaleWizardComponent } from './salewizard.component';

// Import existing form components instead of duplicated step components
import { Userform } from '../usermanagement/userform/userform';
import { Saleorderform } from '../saleordermanagement/saleorderform/saleorderform';
import { Saleorderitemform } from '../saleorderitemmanagement/saleorderitemform/saleorderitemform';
import { Step4PaymentComponent } from './steps/step4-payment/step4-payment.component';
import { Step5InvoiceComponent } from './steps/step5-invoice/step5-invoice.component';

export const SALE_WIZARD_ROUTES: Routes = [
  {
    path: '',
    component: SaleWizardComponent,
    children: [
      {
        path: '',
        redirectTo: 'customer',
        pathMatch: 'full'
      },
      {
        path: 'customer',
        component: Userform,
        data: { step: 1, title: 'Customer', wizardMode: true }
      },
      {
        path: 'order',
        component: Saleorderform,
        data: { step: 2, title: 'Sale Order', wizardMode: true }
      },
      {
        path: 'items',
        component: Saleorderitemform,
        data: { step: 3, title: 'Items', wizardMode: true }
      },
      {
        path: 'payment',
        component: Step4PaymentComponent,
        data: { step: 4, title: 'Payment' }
      },
      {
        path: 'invoice',
        component: Step5InvoiceComponent,
        data: { step: 5, title: 'Invoice' }
      }
    ]
  }
];
