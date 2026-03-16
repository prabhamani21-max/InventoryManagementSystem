// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * JewelleryItem Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-jewelleryitemmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './jewelleryitemmanagement.html',
  styleUrl: './jewelleryitemmanagement.scss',
})
export class Jewelleryitemmanagement {
  // Component serves as a container with router-outlet for child components
}
