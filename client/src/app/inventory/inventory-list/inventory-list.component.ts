import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

import { InventoryItem, ItemQuery } from '../../core/models/inventory-item';
import { InventoryService } from '../../core/services/inventory.service';
import { ItemFormDialogComponent } from '../item-form-dialog/item-form-dialog.component';

@Component({
  selector: 'app-inventory-list',
  templateUrl: './inventory-list.component.html',
  styleUrls: ['./inventory-list.component.scss']
})
export class InventoryListComponent implements OnInit, OnDestroy {
  displayedColumns = ['sku', 'name', 'category', 'location', 'quantityOnHand', 'reorderLevel', 'actions'];
  items: InventoryItem[] = [];
  total = 0;
  categories: string[] = [];

  search = new FormControl('');
  category = new FormControl('');
  query: ItemQuery = { page: 1, pageSize: 25 };

  private destroy$ = new Subject<void>();

  constructor(private inventory: InventoryService, private dialog: MatDialog) { }

  ngOnInit() {
    this.inventory.getCategories().subscribe(c => this.categories = c);

    this.search.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(value => {
        this.query = { ...this.query, search: value, page: 1 };
        this.load();
      });

    this.category.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(value => {
        this.query = { ...this.query, category: value, page: 1 };
        this.load();
      });

    this.load();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onPage(e: PageEvent) {
    this.query = { ...this.query, page: e.pageIndex + 1, pageSize: e.pageSize };
    this.load();
  }

  openForm(item?: InventoryItem) {
    this.dialog.open(ItemFormDialogComponent, { data: item || null, width: '600px' })
      .afterClosed()
      .subscribe(saved => {
        if (saved) {
          this.load();
        }
      });
  }

  load() {
    this.inventory.getItems(this.query).subscribe(result => {
      this.items = result.items;
      this.total = result.total;
    });
  }
}
