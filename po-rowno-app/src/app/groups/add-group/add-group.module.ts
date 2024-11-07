import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { AddGroupPageRoutingModule } from './add-group-routing.module';

import { AddGroupPage } from './add-group.page';
import { ErrorLabelComponent } from 'src/app/_common/components/error-label/error-label.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    AddGroupPageRoutingModule,
    ReactiveFormsModule,
    ErrorLabelComponent,
    TranslateModule,
  ],
  declarations: [AddGroupPage],
})
export class AddGroupPageModule {}
