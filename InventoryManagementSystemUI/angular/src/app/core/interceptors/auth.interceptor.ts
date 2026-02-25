import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private cookieService: CookieService) {}
  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler,
  ): Observable<HttpEvent<unknown>> {
    const token = this.cookieService.get('JewelleryManagement_AUTH_TOKEN');
    // console.log('Auth token:', token);
    // Clone and modify request
    const authReq = token
      ? request.clone({
          setHeaders: { Authorization: `Bearer ${token}` },
        })
      : request;

    return next.handle(authReq);
  }
}
