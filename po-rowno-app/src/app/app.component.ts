import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { addIcons } from 'ionicons';
import localePl from '@angular/common/locales/pl';
import localeEn from '@angular/common/locales/en';
import { registerLocaleData } from '@angular/common';

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
    registerLocaleData(localePl);
    registerLocaleData(localeEn);
  }
}
