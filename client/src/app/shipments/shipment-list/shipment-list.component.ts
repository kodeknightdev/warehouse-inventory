import { Component, OnInit } from '@angular/core';

import { Shipment, ShipmentStatus } from '../../core/models/shipment';
import { ShipmentService } from '../../core/services/shipment.service';

@Component({
  selector: 'app-shipment-list',
  templateUrl: './shipment-list.component.html',
  styleUrls: ['./shipment-list.component.scss']
})
export class ShipmentListComponent implements OnInit {
  displayedColumns = ['reference', 'direction', 'status', 'carrier', 'destination', 'units', 'createdAt'];
  shipments: Shipment[] = [];
  status: ShipmentStatus | '' = '';

  readonly statuses: ShipmentStatus[] = ['Pending', 'Picking', 'Packed', 'InTransit', 'Delivered', 'Cancelled'];

  constructor(private shipmentService: ShipmentService) { }

  ngOnInit() {
    this.load();
  }

  load() {
    this.shipmentService.getShipments(this.status || undefined)
      .subscribe(list => this.shipments = list);
  }
}
