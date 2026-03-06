// angular import
import { Component, inject } from '@angular/core';
import { animate, style, transition, trigger } from '@angular/animations';

// bootstrap import
import { NgbDropdownConfig, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, of } from 'rxjs';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';


@Component({
  selector: 'app-nav-right',
  imports: [SharedModule, ],
  templateUrl: './nav-right.component.html',
  styleUrls: ['./nav-right.component.scss'],
  providers: [NgbDropdownConfig],
  animations: [
    trigger('slideInOutLeft', [
      transition(':enter', [style({ transform: 'translateX(100%)' }), animate('300ms ease-in', style({ transform: 'translateX(0%)' }))]),
      transition(':leave', [animate('300ms ease-in', style({ transform: 'translateX(100%)' }))])
    ]),
    trigger('slideInOutRight', [
      transition(':enter', [style({ transform: 'translateX(-100%)' }), animate('300ms ease-in', style({ transform: 'translateX(0%)' }))]),
      transition(':leave', [animate('300ms ease-in', style({ transform: 'translateX(-100%)' }))])
    ])
  ]
})
export class NavRightComponent {
  element: any;
  profileImageUrl: string = 'assets/images/users/dummy-avatar.jpg';
  currentUser: any;

  router = inject(Router);
  store = inject(Store);
  offcanvasService = inject(NgbOffcanvas);
  // Add these new properties
  userName$: Observable<string> = of('');
  userInitials: string = '';
  userFullName: string = '';
  userId:number=0;


  private authService = inject(AuthenticationService);
  private userService = inject(UserService);
  // constructor
  constructor() {

  }

  ngOnInit(): void {
    this.element = document.documentElement;
    this.loadUserData();
    this.userService.currentUser$.subscribe((user: any) => {
      this.currentUser = user;
     //this.profileImageUrl = user?.profileImageUrl || this.profileImageUrl;
    });

    // Initial load if user is already logged in
    if (this.authService.isAuthenticated()) {
      this.userService.getCurrentUser().subscribe();
    }

    // if (!this.signalRService.connectionEstablished.value) {
    // this.signalRService.startConnection('');
  // }
  }
  // Add this new method
  private loadUserData(): void {
    const decodedToken = this.authService.getUserInformation();
    if (decodedToken) {
      this.userFullName = decodedToken.name || 'User';
      // this.userInitials = this.getInitials(this.userFullName);
      this.userId=+decodedToken.userId;
    }
  }
  handleImageError(event: Event): void {
    const imgElement = event.target as HTMLImageElement;
    imgElement.src = 'assets/images/users/dummy-avatar.jpg';
  }
  goToProfile() {
  this.router.navigate(['/jewelleryManagement/admin/role']);
}
  // public method
  // eslint-disable-next-line
  
  logout() {
    this.authService.logout();
  }

}
