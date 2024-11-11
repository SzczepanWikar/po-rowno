import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { ChangePasswordPageRoutingModule } from './change-password-routing.module';

import { TranslateModule } from '@ngx-translate/core';
import { ErrorLabelComponent } from 'src/app/_common/components/error-label/error-label.component';
import { ChangePasswordPage } from './change-password.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    ChangePasswordPageRoutingModule,
    ReactiveFormsModule,
    TranslateModule,
    ErrorLabelComponent,
  ],
  declarations: [ChangePasswordPage],
})
export class ChangePasswordPageModule {}
