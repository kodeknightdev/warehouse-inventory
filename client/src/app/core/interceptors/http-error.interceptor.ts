import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {

  constructor(private snackBar: MatSnackBar) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err: HttpErrorResponse) => {
        this.snackBar.open(this.describe(err), 'Dismiss', { duration: 6000, panelClass: 'snack-error' });
        return throwError(err);
      })
    );
  }

  private describe(err: HttpErrorResponse): string {
    if (err.status === 0) {
      return 'Can\'t reach the API - is it running?';
    }

    const body = err.error;
    if (typeof body === 'string' && body) {
      return body;
    }
    if (body && body.modelState) {
      // web api model state: { "request.Sku": ["The Sku field is required."] }
      const first = Object.keys(body.modelState)[0];
      return body.modelState[first][0];
    }
    if (body && body.message) {
      return body.message;
    }
    return `Request failed (${err.status})`;
  }
}
