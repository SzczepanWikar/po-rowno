import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { PayPageRoutingModule } from './pay-routing.module';

import { PayPage } from './pay.page';
import { TranslateModule } from '@ngx-translate/core';
import { ErrorLabelComponent } from 'src/app/_common/components/error-label/error-label.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    PayPageRoutingModule,
    TranslateModule,
    ReactiveFormsModule,
    ErrorLabelComponent,
  ],
  declarations: [PayPage],
})
export class PayPageModule {}
