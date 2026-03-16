// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Metal Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-metalmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './metalmanagement.html',
  styleUrl: './metalmanagement.scss',
})
export class Metalmanagement {
  // Component serves as a container with router-outlet for child components
}
