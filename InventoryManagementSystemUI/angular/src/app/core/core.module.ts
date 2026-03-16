import { NgModule, Optional, SkipSelf } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { AuthenticationService } from './services/auth.service';
import { CookieService } from 'ngx-cookie-service';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { LoadingService } from './services/loading.service';
import { ErrorInterceptor } from './interceptors/error.interceptor';

@NgModule({
  providers: [
    AuthenticationService,
    LoadingService,
    CookieService,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ],
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error(
        'CoreModule is already loaded. Import it in AppConfig only.',
      );
    }
  }
}
