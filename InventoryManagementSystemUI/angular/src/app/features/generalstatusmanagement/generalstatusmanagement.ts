// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * General Status Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-generalstatusmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './generalstatusmanagement.html',
  styleUrl: './generalstatusmanagement.scss',
})
export class GeneralStatusmanagement {
  // Component serves as a container with router-outlet for child components
}