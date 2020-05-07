import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, takeUntil } from 'rxjs/operators';

import { Shipment, ShipmentStatus } from '../../core/models/shipment';
import { ShipmentService } from '../../core/services/shipment.service';

@Component({
  selector: 'app-shipment-list',
  templateUrl: './shipment-list.component.html',
  styleUrls: ['./shipment-list.component.scss']
})
export class ShipmentListComponent implements OnInit, OnDestroy {
  displayedColumns = ['reference', 'direction', 'status', 'carrier', 'destination', 'units', 'createdAt'];
  shipments: Shipment[] = [];
  status: ShipmentStatus | '' = '';
  carrier = '';
  carriers: string[] = [];
  loading = false;
  search = new FormControl('');

  private destroy$ = new Subject<void>();

  readonly statuses: ShipmentStatus[] = ['Pending', 'Picking', 'Packed', 'InTransit', 'Delivered', 'Cancelled'];

  constructor(private shipmentService: ShipmentService) { }

  ngOnInit() {
    this.shipmentService.getCarriers().subscribe(c => this.carriers = c);
    this.search.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(() => this.load());
    this.load();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  load() {
    this.loading = true;
    this.shipmentService.getShipments({
      status: this.status || undefined,
      carrier: this.carrier,
      search: this.search.value
    })
      .pipe(finalize(() => this.loading = false))
      .subscribe(list => this.shipments = list);
  }
}
