import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, timer } from 'rxjs';
import { catchError, shareReplay, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import { environment } from '../../../environments/environment';
import { LowStockAlert } from '../models/low-stock-alert';

const POLL_MS = 60 * 1000;

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private baseUrl = `${environment.apiUrl}/alerts`;

  // shared between the banner and the toolbar badge so we only poll once
  readonly openAlerts$: Observable<LowStockAlert[]> = timer(0, POLL_MS).pipe(
    switchMap(() => this.getOpenAlerts().pipe(catchError(() => of([])))),
    shareReplay(1)
  );

  constructor(private http: HttpClient) { }

  getOpenAlerts(): Observable<LowStockAlert[]> {
    return this.http.get<LowStockAlert[]>(this.baseUrl);
  }
}
