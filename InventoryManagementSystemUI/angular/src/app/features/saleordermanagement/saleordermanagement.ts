// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * SaleOrder Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-saleordermanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './saleordermanagement.html',
  styleUrl: './saleordermanagement.scss',
})
export class Saleordermanagement {
  // Component serves as a container with router-outlet for child components
}
