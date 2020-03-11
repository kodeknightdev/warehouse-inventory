import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { ShipmentService } from './shipment.service';

describe('ShipmentService', () => {
  let service: ShipmentService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(ShipmentService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('lists all shipments without a status filter', () => {
    service.getShipments().subscribe(list => expect(list.length).toBe(1));

    const req = http.expectOne(r => r.url === '/api/shipments');
    expect(req.request.params.keys().length).toBe(0);
    req.flush([{ id: 1, reference: 'SHP-1', status: 'Pending' }]);
  });

  it('filters by status', () => {
    service.getShipments('InTransit').subscribe();

    const req = http.expectOne(r => r.url === '/api/shipments');
    expect(req.request.params.get('status')).toBe('InTransit');
    req.flush([]);
  });
});
