// angular import
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// 3rd party import

import {  NgApexchartsModule } from 'ng-apexcharts';
@Component({
  selector: 'app-dash-analytics',
  imports: [SharedModule, NgApexchartsModule, RouterLink],
  templateUrl: './dash-analytics.component.html',
  styleUrls: ['./dash-analytics.component.scss']
})
export class DashAnalyticsComponent {
  // public props


  // constructor
  constructor() {

}
}
