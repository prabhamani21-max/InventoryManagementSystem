// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * Stone Rate History Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-stoneratehistorymanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './stoneratehistorymanagement.html',
  styleUrl: './stoneratehistorymanagement.scss',
})
export class Stoneratehistorymanagement {
  // Component serves as a container with router-outlet for child components
}
