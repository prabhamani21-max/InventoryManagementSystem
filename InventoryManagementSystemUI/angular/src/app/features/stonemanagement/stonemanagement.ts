// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Stone Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-stonemanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './stonemanagement.html',
  styleUrl: './stonemanagement.scss',
})
export class Stonemanagement {
  // Component serves as a container with router-outlet for child components
}
