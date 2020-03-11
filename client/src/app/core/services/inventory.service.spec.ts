import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { InventoryService } from './inventory.service';

describe('InventoryService', () => {
  let service: InventoryService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(InventoryService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('sends paging params with defaults', () => {
    service.getItems().subscribe();

    const req = http.expectOne(r => r.url === '/api/items');
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('pageSize')).toBe('25');
    expect(req.request.params.has('search')).toBe(false);
    req.flush({ items: [], total: 0, page: 1, pageSize: 25 });
  });

  it('only sends search and category when set', () => {
    service.getItems({ search: 'box', category: 'Packaging', page: 2, pageSize: 10 }).subscribe();

    const req = http.expectOne(r => r.url === '/api/items');
    expect(req.request.params.get('search')).toBe('box');
    expect(req.request.params.get('category')).toBe('Packaging');
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], total: 0, page: 2, pageSize: 10 });
  });

  it('posts adjustments to the item', () => {
    service.adjustStock(7, -3, 'damaged').subscribe();

    const req = http.expectOne('/api/items/7/adjust');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ delta: -3, reason: 'damaged' });
    req.flush({});
  });
});
