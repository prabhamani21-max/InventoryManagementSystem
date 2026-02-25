// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * InvoicePayment Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-invoicepaymentmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './invoicepaymentmanagement.html',
  styleUrl: './invoicepaymentmanagement.scss',
})
export class Invoicepaymentmanagement {
  // Component serves as a container with router-outlet for child components
}
