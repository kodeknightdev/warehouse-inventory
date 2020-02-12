import { Component, OnInit } from '@angular/core';

import { InventoryItem } from '../../core/models/inventory-item';
import { InventoryService } from '../../core/services/inventory.service';

@Component({
  selector: 'app-inventory-list',
  templateUrl: './inventory-list.component.html',
  styleUrls: ['./inventory-list.component.scss']
})
export class InventoryListComponent implements OnInit {
  displayedColumns = ['sku', 'name', 'category', 'location', 'quantityOnHand', 'reorderLevel'];
  items: InventoryItem[] = [];
  total = 0;

  constructor(private inventory: InventoryService) { }

  ngOnInit() {
    this.inventory.getItems().subscribe(result => {
      this.items = result.items;
      this.total = result.total;
    });
  }
}
