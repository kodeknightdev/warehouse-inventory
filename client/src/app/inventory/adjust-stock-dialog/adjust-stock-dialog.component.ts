import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { InventoryItem } from '../../core/models/inventory-item';
import { InventoryService } from '../../core/services/inventory.service';

@Component({
  selector: 'app-adjust-stock-dialog',
  templateUrl: './adjust-stock-dialog.component.html'
})
export class AdjustStockDialogComponent {
  saving = false;
  conflict = false;

  form = this.fb.group({
    direction: ['in'],
    quantity: [1, [Validators.required, Validators.min(1)]],
    reason: ['', Validators.maxLength(200)]
  });

  constructor(
    private fb: FormBuilder,
    private inventory: InventoryService,
    private dialogRef: MatDialogRef<AdjustStockDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public item: InventoryItem
  ) { }

  get resulting(): number {
    const v = this.form.value;
    const qty = Number(v.quantity) || 0;
    return this.item.quantityOnHand + (v.direction === 'in' ? qty : -qty);
  }

  save() {
    if (this.form.invalid || this.resulting < 0) {
      return;
    }

    const v = this.form.value;
    const delta = v.direction === 'in' ? v.quantity : -v.quantity;

    this.saving = true;
    this.conflict = false;
    this.inventory.adjustStock(this.item.id, delta, v.reason).subscribe(
      updated => this.dialogRef.close(updated),
      (err: HttpErrorResponse) => {
        this.saving = false;
        if (err.status === 409) {
          // someone else moved stock in the meantime - show the fresh number and let them retry
          this.conflict = true;
          this.inventory.getItem(this.item.id).subscribe(fresh => this.item = fresh);
        }
      }
    );
  }
}
