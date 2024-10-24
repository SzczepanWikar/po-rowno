import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { addIcons } from 'ionicons';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent {
  constructor(translate: TranslateService) {
    addIcons({
      logo: './../assets/icon/logo.svg',
    });

    this.localize(translate);
  }

  private localize(translate: TranslateService) {
    translate.addLangs(['en', 'pl']);
    translate.setDefaultLang('en');
    translate.use(navigator.language === 'pl-PL' ? 'pl' : 'en');
  }
}
