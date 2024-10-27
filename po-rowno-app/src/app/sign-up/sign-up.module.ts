import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { SignUpPageRoutingModule } from './sign-up-routing.module';

import { SignUpPage } from './sign-up.page';
import { AuthService } from '../_services/auth/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { ErrorLabelComponent } from '../_common/components/error-label/error-label.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SignUpPageRoutingModule,
    ReactiveFormsModule,
    TranslateModule,
    ErrorLabelComponent,
  ],
  declarations: [SignUpPage],
  providers: [AuthService],
})
export class SignUpPageModule {}
