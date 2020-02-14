import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { InventoryItem, ItemQuery, PagedResult, SaveItemRequest } from '../models/inventory-item';
import { Location } from '../models/location';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private baseUrl = `${environment.apiUrl}/items`;

  constructor(private http: HttpClient) { }

  getItems(query: ItemQuery = {}): Observable<PagedResult<InventoryItem>> {
    let params = new HttpParams()
      .set('page', String(query.page || 1))
      .set('pageSize', String(query.pageSize || 25));
    if (query.search) {
      params = params.set('search', query.search);
    }
    if (query.category) {
      params = params.set('category', query.category);
    }
    return this.http.get<PagedResult<InventoryItem>>(this.baseUrl, { params });
  }

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/categories`);
  }

  getItem(id: number): Observable<InventoryItem> {
    return this.http.get<InventoryItem>(`${this.baseUrl}/${id}`);
  }

  createItem(item: SaveItemRequest): Observable<InventoryItem> {
    return this.http.post<InventoryItem>(this.baseUrl, item);
  }

  updateItem(id: number, item: SaveItemRequest): Observable<InventoryItem> {
    return this.http.put<InventoryItem>(`${this.baseUrl}/${id}`, item);
  }

  // TODO: move to its own service if locations get more screens
  getLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(`${environment.apiUrl}/locations`);
  }
}
