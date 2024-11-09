import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { EditGroupPageRoutingModule } from './edit-group-routing.module';

import { EditGroupPage } from './edit-group.page';
import { TranslateModule } from '@ngx-translate/core';
import { ErrorLabelComponent } from 'src/app/_common/components/error-label/error-label.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    ReactiveFormsModule,
    ErrorLabelComponent,
    TranslateModule,
    EditGroupPageRoutingModule,
  ],
  declarations: [EditGroupPage],
})
export class EditGroupPageModule {}
