import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';

import { InventoryItem } from '../../core/models/inventory-item';
import { Location } from '../../core/models/location';
import { InventoryService } from '../../core/services/inventory.service';

@Component({
  selector: 'app-item-form-dialog',
  templateUrl: './item-form-dialog.component.html',
  styleUrls: ['./item-form-dialog.component.scss']
})
export class ItemFormDialogComponent implements OnInit {
  form: FormGroup;
  locations$: Observable<Location[]>;
  saving = false;
  isNew: boolean;

  readonly units = ['ea', 'box', 'roll', 'pallet', 'kg', 'm'];

  constructor(
    private fb: FormBuilder,
    private inventory: InventoryService,
    private dialogRef: MatDialogRef<ItemFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public item: InventoryItem | null
  ) {
    this.isNew = !item;
  }

  ngOnInit() {
    this.locations$ = this.inventory.getLocations();

    const i = this.item;
    this.form = this.fb.group({
      sku: [i ? i.sku : '', [Validators.required, Validators.maxLength(32)]],
      name: [i ? i.name : '', [Validators.required, Validators.maxLength(200)]],
      description: [i ? i.description : ''],
      category: [i ? i.category : ''],
      quantityOnHand: [{ value: i ? i.quantityOnHand : 0, disabled: !this.isNew }, [Validators.min(0)]],
      reorderLevel: [i ? i.reorderLevel : 0, [Validators.min(0)]],
      unitOfMeasure: [i ? i.unitOfMeasure : 'ea', Validators.required],
      unitCost: [i ? i.unitCost : 0, [Validators.min(0)]],
      locationId: [i ? i.locationId : null]
    });
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    const value = this.form.getRawValue();
    const request = this.isNew
      ? this.inventory.createItem(value)
      : this.inventory.updateItem(this.item.id, value);

    request.subscribe(
      saved => this.dialogRef.close(saved),
      () => this.saving = false
    );
  }
}
