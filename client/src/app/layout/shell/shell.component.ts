import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import { AlertService } from '../../core/services/alert.service';

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent {
  @Input() title = '';

  alertCount$: Observable<number> = this.alertService.openAlerts$.pipe(map(a => a.length));

  // tablets on the floor - collapse the nav into an overlay below ~900px
  isSmall$: Observable<boolean> = this.breakpoints.observe('(max-width: 900px)').pipe(
    map(r => r.matches),
    shareReplay(1)
  );

  constructor(private alertService: AlertService, private breakpoints: BreakpointObserver) { }
}
