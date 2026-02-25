// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Role Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-rolemanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './rolemanagement.html',
  styleUrl: './rolemanagement.scss',
})
export class Rolemanagement {
  // Component serves as a container with router-outlet for child components
}
