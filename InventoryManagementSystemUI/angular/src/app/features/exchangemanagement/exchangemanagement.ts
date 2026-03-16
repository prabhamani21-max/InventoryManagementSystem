// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Exchange Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-exchangemanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './exchangemanagement.html',
  styleUrl: './exchangemanagement.scss',
})
export class Exchangemanagement {
  // Component serves as a container with router-outlet for child components
}