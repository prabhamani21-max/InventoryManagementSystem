// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Invoice Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-invoicemanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './invoicemanagement.html',
  styleUrl: './invoicemanagement.scss',
})
export class Invoicemanagement {
  // Component serves as a container with router-outlet for child components
}