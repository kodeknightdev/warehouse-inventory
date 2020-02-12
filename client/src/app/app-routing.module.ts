import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { InventoryListComponent } from './inventory/inventory-list/inventory-list.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'inventory' },
  { path: 'inventory', component: InventoryListComponent },
  { path: '**', redirectTo: 'inventory' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
