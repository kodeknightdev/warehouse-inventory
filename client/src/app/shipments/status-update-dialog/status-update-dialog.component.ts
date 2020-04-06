import { Component, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { NEXT_STATUSES, Shipment, ShipmentStatus } from '../../core/models/shipment';
import { ShipmentService } from '../../core/services/shipment.service';

@Component({
  selector: 'app-status-update-dialog',
  templateUrl: './status-update-dialog.component.html'
})
export class StatusUpdateDialogComponent {
  options: ShipmentStatus[];
  saving = false;

  form = this.fb.group({
    status: [null, Validators.required],
    location: [''],
    note: ['']
  });

  constructor(
    private fb: FormBuilder,
    private shipments: ShipmentService,
    private dialogRef: MatDialogRef<StatusUpdateDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public shipment: Shipment
  ) {
    this.options = NEXT_STATUSES[shipment.status] || [];
    if (this.options.length) {
      this.form.patchValue({ status: this.options[0] });
    }
  }

  label(s: ShipmentStatus): string {
    if (s === 'InTransit') {
      return this.shipment.status === 'InTransit' ? 'Scan update (still in transit)' : 'In transit';
    }
    return s;
  }

  save() {
    if (this.form.invalid) {
      return;
    }
    const v = this.form.value;
    this.saving = true;
    this.shipments.updateStatus(this.shipment.id, v.status, v.location, v.note).subscribe(
      updated => this.dialogRef.close(updated),
      () => this.saving = false
    );
  }
}
