// angular import
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { DecodedToken } from 'src/app/core/models/decoded-token.model';

@Component({
  selector: 'app-sales-dashboard',
  imports: [SharedModule, CommonModule, RouterModule],
  templateUrl: './salesdashboard.component.html',
  styleUrls: ['./salesdashboard.component.scss']
})
export class SalesDashboardComponent implements OnInit {
  // public props
  currentUser: DecodedToken | null = null;
  activeTab: string = 'overview';

  // constructor
  constructor(
    private authService: AuthenticationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    
    // Track route changes to update active tab
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateActiveTab();
    });
    
    // Set initial active tab
    this.updateActiveTab();
  }

  updateActiveTab(): void {
    const url = this.router.url;
    if (url.includes('/orders')) {
      this.activeTab = 'orders';
    } else if (url.includes('/invoices')) {
      this.activeTab = 'invoices';
    } else if (url.includes('/customers')) {
      this.activeTab = 'customers';
    } else if (url.includes('/exchanges')) {
      this.activeTab = 'exchanges';
    } else {
      this.activeTab = 'overview';
    }
  }
}
