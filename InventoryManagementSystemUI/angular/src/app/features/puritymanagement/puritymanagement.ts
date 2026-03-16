// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Purity Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-puritymanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './puritymanagement.html',
  styleUrl: './puritymanagement.scss',
})
export class Puritymanagement {
  // Component serves as a container with router-outlet for child components
}
