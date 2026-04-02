// Angular Import
import { Component, OnInit, inject } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';

// project import
import { AuthenticationService } from './core/services/auth.service';
import { SpinnerComponent } from './theme/shared/components/spinner/spinner.component';

@Component({
  selector: 'app-root',
  imports: [RouterModule, SpinnerComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthenticationService);

  // life cycle event
  ngOnInit() {
    this.authService.initializeSession();
    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
      window.scrollTo(0, 0);
    });
  }
}
