// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Warehouse Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-warehousemanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './warehousemanagement.html',
  styleUrl: './warehousemanagement.scss',
})
export class Warehousemanagement {
  // Component serves as a container with router-outlet for child components
}
