// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * InvoiceItem Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-invoiceitemmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './invoiceitemmanagement.html',
  styleUrl: './invoiceitemmanagement.scss',
})
export class Invoiceitemmanagement {
  // Component serves as a container with router-outlet for child components
}
