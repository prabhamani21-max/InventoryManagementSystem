// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * TCS Management Component
 * Main container component that hosts the TCS report and dashboard components
 */
@Component({
  selector: 'app-tcsmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './tcsmanagement.html',
  styleUrl: './tcsmanagement.scss'
})
export class Tcsmanagement {
  // Component serves as a container with router-outlet for child components
}
