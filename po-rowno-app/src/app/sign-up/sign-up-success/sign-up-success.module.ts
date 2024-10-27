import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { SignUpSuccessPageRoutingModule } from './sign-up-success-routing.module';

import { SignUpSuccessPage } from './sign-up-success.page';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SignUpSuccessPageRoutingModule,
    TranslateModule,
  ],
  declarations: [SignUpSuccessPage],
})
export class SignUpSuccessPageModule {}
