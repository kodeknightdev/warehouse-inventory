import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

import { InventoryItem, StockMovement } from '../../core/models/inventory-item';
import { InventoryService } from '../../core/services/inventory.service';
import { AdjustStockDialogComponent } from '../adjust-stock-dialog/adjust-stock-dialog.component';
import { ItemFormDialogComponent } from '../item-form-dialog/item-form-dialog.component';

@Component({
  selector: 'app-item-detail',
  templateUrl: './item-detail.component.html',
  styleUrls: ['./item-detail.component.scss']
})
export class ItemDetailComponent implements OnInit {
  item: InventoryItem;
  movements: StockMovement[] = [];
  movementColumns = ['createdAt', 'delta', 'quantityAfter', 'reason'];

  constructor(
    private route: ActivatedRoute,
    private inventory: InventoryService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.load();
  }

  load() {
    const id = +this.route.snapshot.paramMap.get('id');
    this.inventory.getItem(id).subscribe(item => this.item = item);
    this.loadMovements(id);
  }

  loadMovements(id: number) {
    this.inventory.getMovements(id).subscribe(m => this.movements = m);
  }

  edit() {
    this.dialog.open(ItemFormDialogComponent, { data: this.item, width: '600px' })
      .afterClosed()
      .subscribe(saved => {
        if (saved) {
          this.item = saved;
        }
      });
  }

  adjust() {
    this.dialog.open(AdjustStockDialogComponent, { data: this.item, width: '420px' })
      .afterClosed()
      .subscribe(updated => {
        if (updated) {
          this.item = updated;
          this.loadMovements(updated.id);
        }
      });
  }
}
