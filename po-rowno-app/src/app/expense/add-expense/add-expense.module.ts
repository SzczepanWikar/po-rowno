import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { AddExpensePageRoutingModule } from './add-expense-routing.module';

import { AddExpensePage } from './add-expense.page';
import { TranslateModule } from '@ngx-translate/core';
import { ErrorLabelComponent } from 'src/app/_common/components/error-label/error-label.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    AddExpensePageRoutingModule,
    TranslateModule,
    ReactiveFormsModule,
    ErrorLabelComponent,
  ],
  declarations: [AddExpensePage],
})
export class AddExpensePageModule {}
