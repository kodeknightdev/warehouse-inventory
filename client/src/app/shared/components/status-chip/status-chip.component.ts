import { Component, Input } from '@angular/core';

import { ShipmentStatus } from '../../../core/models/shipment';

@Component({
  selector: 'app-status-chip',
  template: `<span class="chip" [ngClass]="status | lowercase">{{ label }}</span>`,
  styleUrls: ['./status-chip.component.scss']
})
export class StatusChipComponent {
  @Input() status: ShipmentStatus;

  get label(): string {
    return this.status === 'InTransit' ? 'In transit' : this.status;
  }
}
