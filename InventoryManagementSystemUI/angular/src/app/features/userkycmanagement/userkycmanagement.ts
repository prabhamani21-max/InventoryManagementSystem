// angular import
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

@Component({
  selector: 'app-userkycmanagement',
  imports: [SharedModule, RouterOutlet],
  templateUrl: './userkycmanagement.html',
  styleUrl: './userkycmanagement.scss',
})
export class UserKycManagement {
  // Component serves as a container with router-outlet for child components
}
