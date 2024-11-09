import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { JoinPageRoutingModule } from './join-routing.module';

import { JoinPage } from './join.page';
import { TranslateModule } from '@ngx-translate/core';
import { ErrorLabelComponent } from 'src/app/_common/components/error-label/error-label.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    JoinPageRoutingModule,
    ReactiveFormsModule,
    TranslateModule,
    ErrorLabelComponent,
  ],
  declarations: [JoinPage],
})
export class JoinPageModule {}
