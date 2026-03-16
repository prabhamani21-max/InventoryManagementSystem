import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Observable, finalize } from 'rxjs';
import { LoadingService } from '../services/loading.service';
 
@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private loadingService: LoadingService) {}
 
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    // Delay show() to next tick to avoid ExpressionChangedAfterItHasBeenCheckedError
    setTimeout(() => this.loadingService.show(), 0);
    return next.handle(req).pipe(finalize(() => this.loadingService.hide()));
  }
}
 