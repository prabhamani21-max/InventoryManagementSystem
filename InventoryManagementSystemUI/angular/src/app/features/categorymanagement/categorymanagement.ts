// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Category Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-categorymanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './categorymanagement.html',
  styleUrl: './categorymanagement.scss',
})
export class Categorymanagement {
  // Component serves as a container with router-outlet for child components
}
