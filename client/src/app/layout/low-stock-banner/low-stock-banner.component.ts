import { Component, OnInit } from '@angular/core';

import { LowStockAlert } from '../../core/models/low-stock-alert';
import { AlertService } from '../../core/services/alert.service';

@Component({
  selector: 'app-low-stock-banner',
  templateUrl: './low-stock-banner.component.html',
  styleUrls: ['./low-stock-banner.component.scss']
})
export class LowStockBannerComponent implements OnInit {
  alerts: LowStockAlert[] = [];
  expanded = false;

  constructor(private alertService: AlertService) { }

  ngOnInit() {
    this.alertService.openAlerts$.subscribe(a => this.alerts = a);
  }

  acknowledge(alert: LowStockAlert) {
    this.alertService.acknowledge(alert.id).subscribe(() => {
      alert.acknowledgedAt = new Date().toISOString();
    });
  }

  acknowledgeAll() {
    this.alerts.filter(a => !a.acknowledgedAt).forEach(a => this.acknowledge(a));
  }
}
