import { Component, Input, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-error-label',
  templateUrl: './error-label.component.html',
  styleUrls: ['./error-label.component.scss'],
  standalone: true,
  imports: [TranslateModule],
})
export class ErrorLabelComponent {
  @Input()
  error: string = 'ERROR_OCCURED';

  constructor() {}
}
