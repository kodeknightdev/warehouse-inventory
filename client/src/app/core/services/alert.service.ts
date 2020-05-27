import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of, timer } from 'rxjs';
import { catchError, shareReplay, switchMap } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { SILENT_ERRORS_HEADER } from '../interceptors/http-error.interceptor';
import { LowStockAlert } from '../models/low-stock-alert';

const POLL_MS = 60 * 1000;

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private baseUrl = `${environment.apiUrl}/alerts`;

  // shared between the banner and the toolbar badge so we only poll once.
  // refCount so the timer actually stops when nobody is subscribed - plain shareReplay(1) kept it running forever
  readonly openAlerts$: Observable<LowStockAlert[]> = timer(0, POLL_MS).pipe(
    switchMap(() => this.getOpenAlerts(true).pipe(catchError(() => of([])))),
    shareReplay({ bufferSize: 1, refCount: true })
  );

  constructor(private http: HttpClient) { }

  getOpenAlerts(silent = false): Observable<LowStockAlert[]> {
    const headers = silent ? new HttpHeaders({ [SILENT_ERRORS_HEADER]: '1' }) : undefined;
    return this.http.get<LowStockAlert[]>(this.baseUrl, { headers });
  }

  acknowledge(id: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/acknowledge`, {});
  }
}
