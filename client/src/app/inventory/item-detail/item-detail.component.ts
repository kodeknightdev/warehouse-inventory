import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

import { InventoryItem } from '../../core/models/inventory-item';
import { InventoryService } from '../../core/services/inventory.service';
import { ItemFormDialogComponent } from '../item-form-dialog/item-form-dialog.component';

@Component({
  selector: 'app-item-detail',
  templateUrl: './item-detail.component.html',
  styleUrls: ['./item-detail.component.scss']
})
export class ItemDetailComponent implements OnInit {
  item: InventoryItem;

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
}
