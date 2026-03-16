// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Metal Rate History Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-metalratehistorymanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './metalratehistorymanagement.html',
  styleUrl: './metalratehistorymanagement.scss',
})
export class Metalratehistorymanagement {
  // Component serves as a container with router-outlet for child components
}
