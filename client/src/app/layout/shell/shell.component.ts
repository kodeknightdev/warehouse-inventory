import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { AlertService } from '../../core/services/alert.service';

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent {
  @Input() title = '';

  alertCount$: Observable<number> = this.alertService.openAlerts$.pipe(map(a => a.length));

  constructor(private alertService: AlertService) { }
}
