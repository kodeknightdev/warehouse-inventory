import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Shipment } from '../../core/models/shipment';
import { ShipmentService } from '../../core/services/shipment.service';

@Component({
  selector: 'app-shipment-detail',
  templateUrl: './shipment-detail.component.html',
  styleUrls: ['./shipment-detail.component.scss']
})
export class ShipmentDetailComponent implements OnInit {
  shipment: Shipment;
  lineColumns = ['sku', 'name', 'quantity'];

  constructor(private route: ActivatedRoute, private shipmentService: ShipmentService) { }

  ngOnInit() {
    const id = +this.route.snapshot.paramMap.get('id');
    this.shipmentService.getShipment(id).subscribe(s => this.shipment = s);
  }
}
