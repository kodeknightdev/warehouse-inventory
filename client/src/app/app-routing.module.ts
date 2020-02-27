import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DashboardComponent } from './dashboard/dashboard.component';
import { InventoryListComponent } from './inventory/inventory-list/inventory-list.component';
import { ShipmentListComponent } from './shipments/shipment-list/shipment-list.component';
import { ShipmentFormComponent } from './shipments/shipment-form/shipment-form.component';
import { ShipmentDetailComponent } from './shipments/shipment-detail/shipment-detail.component';
import { ItemDetailComponent } from './inventory/item-detail/item-detail.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'inventory', component: InventoryListComponent },
  { path: 'inventory/:id', component: ItemDetailComponent },
  { path: 'shipments', component: ShipmentListComponent },
  { path: 'shipments/new', component: ShipmentFormComponent },
  { path: 'shipments/:id', component: ShipmentDetailComponent },
  { path: '**', redirectTo: 'dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
