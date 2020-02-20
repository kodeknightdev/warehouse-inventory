import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { DashboardSummary } from '../core/models/dashboard-summary';
import { DashboardService } from '../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  summary$: Observable<DashboardSummary>;

  constructor(private dashboard: DashboardService) { }

  ngOnInit() {
    this.summary$ = this.dashboard.getSummary();
  }
}
