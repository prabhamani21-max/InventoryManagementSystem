// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Payment Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-paymentmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './paymentmanagement.html',
  styleUrl: './paymentmanagement.scss',
})
export class Paymentmanagement {
  // Component serves as a container with router-outlet for child components
}
