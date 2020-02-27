import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';

import { InventoryItem } from '../../core/models/inventory-item';
import { InventoryService } from '../../core/services/inventory.service';
import { ShipmentService } from '../../core/services/shipment.service';

@Component({
  selector: 'app-shipment-form',
  templateUrl: './shipment-form.component.html',
  styleUrls: ['./shipment-form.component.scss']
})
export class ShipmentFormComponent implements OnInit {
  form: FormGroup;
  items: InventoryItem[] = [];
  saving = false;

  readonly carriers = ['UPS', 'FedEx', 'USPS', 'DHL', 'Local courier', 'Customer pickup'];

  constructor(
    private fb: FormBuilder,
    private inventory: InventoryService,
    private shipments: ShipmentService,
    private router: Router
  ) { }

  get lines(): FormArray {
    return this.form.get('lines') as FormArray;
  }

  ngOnInit() {
    this.form = this.fb.group({
      direction: ['Outbound', Validators.required],
      destination: ['', [Validators.required, Validators.maxLength(500)]],
      carrier: [''],
      trackingNumber: [''],
      expectedAt: [null],
      lines: this.fb.array([])
    });
    this.addLine();

    // good enough for now, we have < 100 skus
    this.inventory.getItems({ pageSize: 100 })
      .pipe(map(r => r.items))
      .subscribe(items => this.items = items);
  }

  addLine() {
    this.lines.push(this.fb.group({
      inventoryItemId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(0)]]
    }));
  }

  removeLine(index: number) {
    this.lines.removeAt(index);
  }

  available(itemId: number): number {
    const item = this.items.find(i => i.id === itemId);
    return item ? item.quantityOnHand : null;
  }

  save() {
    if (this.form.invalid || !this.lines.length) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.shipments.create(this.form.value).subscribe(
      created => this.router.navigate(['/shipments', created.id]),
      () => this.saving = false
    );
  }
}
