import { Component, Input, OnChanges } from '@angular/core';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { Label } from 'ng2-charts';

import { CategoryStock } from '../../core/models/dashboard-summary';

@Component({
  selector: 'app-category-chart',
  template: `
    <div class="chart-wrap">
      <canvas baseChart
        [datasets]="datasets"
        [labels]="labels"
        [options]="options"
        chartType="horizontalBar"
        [legend]="false">
      </canvas>
    </div>
  `,
  styles: ['.chart-wrap { position: relative; height: 280px; }']
})
export class CategoryChartComponent implements OnChanges {
  @Input() data: CategoryStock[] = [];

  labels: Label[] = [];
  datasets: ChartDataSets[] = [];

  options: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      xAxes: [{ ticks: { beginAtZero: true, callback: v => '$' + v } }]
    },
    tooltips: {
      callbacks: {
        label: item => '$' + Number(item.xLabel).toFixed(2)
      }
    }
  };

  ngOnChanges() {
    this.labels = this.data.map(d => d.category);
    this.datasets = [{
      data: this.data.map(d => d.value),
      backgroundColor: '#5c6bc0',
      hoverBackgroundColor: '#3949ab'
    }];
  }
}
