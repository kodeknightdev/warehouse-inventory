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
import { ItemDetailComponent } from './inventory/item-detail/item-detail.component';
import { ItemFormDialogComponent } from './inventory/item-form-dialog/item-form-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    ShellComponent,
    InventoryListComponent,
    ItemFormDialogComponent,
    ItemDetailComponent
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
  entryComponents: [ItemFormDialogComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
