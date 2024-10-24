import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { SignInPageRoutingModule } from './sign-in-routing.module';

import { TranslateModule } from '@ngx-translate/core';
import { SignInPage } from './sign-in.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SignInPageRoutingModule,
    TranslateModule,
    ReactiveFormsModule,
  ],
  providers: [],
  declarations: [SignInPage],
})
export class SignInPageModule {}
