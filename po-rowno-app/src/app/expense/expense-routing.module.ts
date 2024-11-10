import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'add',
    loadChildren: () =>
      import('./add-expense/add-expense.module').then(
        (m) => m.AddExpensePageModule,
      ),
  },
  {
    path: 'pay',
    loadChildren: () => import('./pay/pay.module').then((m) => m.PayPageModule),
  },
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forChild(routes)],
})
export class ExpenseRoutingModule {}
