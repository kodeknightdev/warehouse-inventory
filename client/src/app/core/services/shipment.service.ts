import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CreateShipmentRequest, Shipment, ShipmentFilter, ShipmentStatus } from '../models/shipment';

@Injectable({
  providedIn: 'root'
})
export class ShipmentService {
  private baseUrl = `${environment.apiUrl}/shipments`;

  constructor(private http: HttpClient) { }

  getShipments(filter: ShipmentFilter = {}): Observable<Shipment[]> {
    let params = new HttpParams();
    if (filter.status) {
      params = params.set('status', filter.status);
    }
    if (filter.carrier) {
      params = params.set('carrier', filter.carrier);
    }
    if (filter.search) {
      params = params.set('search', filter.search);
    }
    return this.http.get<Shipment[]>(this.baseUrl, { params });
  }

  getCarriers(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/carriers`);
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
