// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

/**
 * ItemStock Management Component
 * Main container component that hosts the table and form components
 */
@Component({
  selector: 'app-itemstockmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './itemstockmanagement.html',
  styleUrl: './itemstockmanagement.scss',
})
export class Itemstockmanagement {
  // Component serves as a container with router-outlet for child components
}