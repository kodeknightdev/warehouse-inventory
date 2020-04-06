import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CreateShipmentRequest, Shipment, ShipmentStatus } from '../models/shipment';

@Injectable({
  providedIn: 'root'
})
export class ShipmentService {
  private baseUrl = `${environment.apiUrl}/shipments`;

  constructor(private http: HttpClient) { }

  getShipments(status?: ShipmentStatus): Observable<Shipment[]> {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<Shipment[]>(this.baseUrl, { params });
  }

  getShipment(id: number): Observable<Shipment> {
    return this.http.get<Shipment>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateShipmentRequest): Observable<Shipment> {
    return this.http.post<Shipment>(this.baseUrl, request);
  }

  updateStatus(id: number, status: ShipmentStatus, location?: string, note?: string): Observable<Shipment> {
    return this.http.post<Shipment>(`${this.baseUrl}/${id}/status`, { status, location, note });
  }
}
