// angular import
import { Component } from '@angular/core';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// 3rd party import

import {  NgApexchartsModule } from 'ng-apexcharts';
@Component({
  selector: 'app-customer-dash',
  imports: [SharedModule, NgApexchartsModule, ],
  templateUrl: './customerdashboard.component.html',
  styleUrls: ['./customerdashboard.component.scss']
})
export class CustomerDashboardComponent {
  // public props


  // constructor
  constructor() {

}
}
