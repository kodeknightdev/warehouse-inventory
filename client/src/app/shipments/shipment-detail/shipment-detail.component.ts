import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

import { NEXT_STATUSES, Shipment } from '../../core/models/shipment';
import { ShipmentService } from '../../core/services/shipment.service';
import { StatusUpdateDialogComponent } from '../status-update-dialog/status-update-dialog.component';

@Component({
  selector: 'app-shipment-detail',
  templateUrl: './shipment-detail.component.html',
  styleUrls: ['./shipment-detail.component.scss']
})
export class ShipmentDetailComponent implements OnInit {
  shipment: Shipment;
  lineColumns = ['sku', 'name', 'quantity'];

  constructor(
    private route: ActivatedRoute,
    private shipmentService: ShipmentService,
    private dialog: MatDialog
  ) { }

  get canUpdate(): boolean {
    return this.shipment && NEXT_STATUSES[this.shipment.status].length > 0;
  }

  ngOnInit() {
    const id = +this.route.snapshot.paramMap.get('id');
    this.shipmentService.getShipment(id).subscribe(s => this.shipment = s);
  }

  updateStatus() {
    this.dialog.open(StatusUpdateDialogComponent, { data: this.shipment, width: '420px' })
      .afterClosed()
      .subscribe(updated => {
        if (updated) {
          this.shipment = updated;
        }
      });
  }
}
