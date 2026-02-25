import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private toastr: ToastrService,
    private router: Router,
  ) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unexpected error occurred.';

        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = `Error: ${error.error.message}`;
        } else {
          // Server-side error
          switch (error.status) {
            case 0:
              errorMessage =
                'Network error: Please check your internet connection.';
              this.toastr.error(errorMessage);
              break;

            case 400:
              errorMessage =
                error.error?.Data?.message ||
                error.error?.Message ||
                'Bad Request: Invalid input.';
              // this.toastr.warning(errorMessage);
              break;

            case 401:
              errorMessage = 'Unauthorized';
              this.toastr.warning(errorMessage);
              break;

            case 403:
              errorMessage =
                'Forbidden: You don’t have permission to perform this action.';
              this.toastr.error(errorMessage);
              break;

            case 404:
              errorMessage =
                'Not Found: The requested resource does not exist.';
              this.toastr.info(errorMessage);
              break;

            case 409:
              errorMessage = 'The resource already exists.';
              this.toastr.warning(errorMessage);
              break;
            case 422:
              errorMessage = 'User inactive Please contact admin ';
              this.toastr.warning(errorMessage);
              break;

            case 500:
              errorMessage = 'Server error: Please try again later.';
              this.toastr.error(errorMessage);
              break;

            default:
              this.toastr.error(
                error.message || errorMessage,
                `Error ${error.status}`,
              );
              break;
          }
        }

        return throwError(() => error);
      }),
    );
  }
}
