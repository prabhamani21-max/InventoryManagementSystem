// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Supplier Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-suppliermanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './suppliermanagement.html',
  styleUrl: './suppliermanagement.scss',
})
export class Suppliermanagement {
  // Component serves as a container with router-outlet for child components
}