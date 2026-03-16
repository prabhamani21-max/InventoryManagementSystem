// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * User Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-usermanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './usermanagement.html',
  styleUrl: './usermanagement.scss',
})
export class Usermanagement {
  // Component serves as a container with router-outlet for child components
}