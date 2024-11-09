import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddExpensePage } from './add-expense/add-expense.page';

const routes: Routes = [
  {
    path: 'add',
    loadChildren: () =>
      import('./add-expense/add-expense.module').then(
        (m) => m.AddExpensePageModule,
      ),
  },
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forChild(routes)],
})
export class ExpenseRoutingModule {}
