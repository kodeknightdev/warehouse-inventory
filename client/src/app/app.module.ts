import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MaterialModule } from './material.module';
import { ShellComponent } from './layout/shell/shell.component';
import { InventoryListComponent } from './inventory/inventory-list/inventory-list.component';
import { AdjustStockDialogComponent } from './inventory/adjust-stock-dialog/adjust-stock-dialog.component';
import { ItemDetailComponent } from './inventory/item-detail/item-detail.component';
import { ItemFormDialogComponent } from './inventory/item-form-dialog/item-form-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    ShellComponent,
    InventoryListComponent,
    ItemFormDialogComponent,
    ItemDetailComponent,
    AdjustStockDialogComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    AppRoutingModule,
    MaterialModule
  ],
  providers: [],
  entryComponents: [ItemFormDialogComponent, AdjustStockDialogComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
