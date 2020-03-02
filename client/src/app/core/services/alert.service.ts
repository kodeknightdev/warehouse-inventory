import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { LowStockAlert } from '../models/low-stock-alert';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private baseUrl = `${environment.apiUrl}/alerts`;

  constructor(private http: HttpClient) { }

  getOpenAlerts(): Observable<LowStockAlert[]> {
    return this.http.get<LowStockAlert[]>(this.baseUrl);
  }
}
