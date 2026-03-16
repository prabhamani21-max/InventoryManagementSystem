// angular import
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// 3rd party import
import { NgApexchartsModule } from 'ng-apexcharts';

// service import
import { UserKycService } from 'src/app/core/services/user-kyc.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { UserKyc } from 'src/app/core/models/user-kyc.model';
import { DecodedToken } from 'src/app/core/models/decoded-token.model';

@Component({
  selector: 'app-customer-dash',
  imports: [SharedModule, NgApexchartsModule, CommonModule, RouterModule],
  templateUrl: './customerdashboard.component.html',
  styleUrls: ['./customerdashboard.component.scss']
})
export class CustomerDashboardComponent implements OnInit {
  // public props
  userKyc: UserKyc | null = null;
  isLoading = true;
  hasKyc = false;
  currentUser: DecodedToken | null = null;
  activeTab: string = 'kyc';

  // constructor
  constructor(
    private userKycService: UserKycService,
    private authService: AuthenticationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    if (this.currentUser && this.currentUser.userId) {
      this.loadUserKyc();
    } else {
      this.isLoading = false;
    }
    
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
    } else {
      this.activeTab = 'kyc';
    }
  }

  loadUserKyc(): void {
    const userId = Number(this.currentUser?.userId);
    if (!userId) {
      this.isLoading = false;
      return;
    }

    this.isLoading = true;
    this.userKycService.getUserKycByUserId(userId).subscribe({
      next: (kyc) => {
        this.userKyc = kyc;
        this.hasKyc = kyc !== null;
        this.isLoading = false;
      },
      error: (error) => {
        // 404 means no KYC found for this user
        if (error.status === 404) {
          this.userKyc = null;
          this.hasKyc = false;
        } else {
          console.error('Error loading KYC:', error);
        }
        this.isLoading = false;
      }
    });
  }

  // Helper method to mask Aadhaar number for display
  getMaskedAadhaar(): string {
    if (!this.userKyc?.aadhaarCardNumber) return '-';
    const aadhaar = this.userKyc.aadhaarCardNumber;
    if (aadhaar.length < 8) return aadhaar;
    return aadhaar.substring(0, 4) + 'XXXX' + aadhaar.substring(aadhaar.length - 4);
  }

  // Helper method to mask PAN number for display
  getMaskedPan(): string {
    if (!this.userKyc?.panCardNumber) return '-';
    const pan = this.userKyc.panCardNumber;
    if (pan.length < 6) return pan;
    return pan.substring(0, 2) + 'XXXX' + pan.substring(pan.length - 2);
  }

  // Get verification status label
  getVerificationLabel(): string {
    return this.userKyc?.isVerified ? 'Verified' : 'Pending Verification';
  }

  // Get verification status class
  getVerificationClass(): string {
    return this.userKyc?.isVerified ? 'badge bg-success' : 'badge bg-warning';
  }
}
