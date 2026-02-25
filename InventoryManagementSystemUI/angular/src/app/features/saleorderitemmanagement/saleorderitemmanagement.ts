// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * SaleOrderItem Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-saleorderitemmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './saleorderitemmanagement.html',
  styleUrl: './saleorderitemmanagement.scss',
})
export class Saleorderitemmanagement {
  // Component serves as a container with router-outlet for child components
}
