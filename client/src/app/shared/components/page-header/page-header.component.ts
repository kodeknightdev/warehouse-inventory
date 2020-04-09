import { Component, Input } from '@angular/core';

/**
 * Title row used at the top of the list pages. Anything projected goes on the right (buttons etc).
 */
@Component({
  selector: 'app-page-header',
  template: `
    <div class="page-header">
      <h2>{{ title }}</h2>
      <span class="sub" *ngIf="subtitle">{{ subtitle }}</span>
      <span class="spacer"></span>
      <ng-content></ng-content>
    </div>
  `,
  styles: [`
    .page-header { display: flex; align-items: center; margin-bottom: 16px; }
    h2 { margin: 0 12px 0 0; }
    .sub { color: #78909c; }
    .spacer { flex: 1; }
  `]
})
export class PageHeaderComponent {
  @Input() title: string;
  @Input() subtitle: string;
}
