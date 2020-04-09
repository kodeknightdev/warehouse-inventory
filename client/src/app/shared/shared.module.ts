import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { MaterialModule } from './material.module';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { StatusChipComponent } from './components/status-chip/status-chip.component';

const components = [
  PageHeaderComponent,
  StatusChipComponent
];

@NgModule({
  declarations: components,
  imports: [CommonModule],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    MaterialModule,
    ...components
  ]
})
export class SharedModule { }
