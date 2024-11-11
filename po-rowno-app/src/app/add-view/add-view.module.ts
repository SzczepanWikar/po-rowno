import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { AddViewPageRoutingModule } from './add-view-routing.module';

import { AddViewPage } from './add-view.page';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    AddViewPageRoutingModule,
    TranslateModule,
  ],
  declarations: [AddViewPage],
})
export class AddViewPageModule {}
