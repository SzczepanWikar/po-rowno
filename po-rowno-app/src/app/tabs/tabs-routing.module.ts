import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TabsPage } from './tabs.page';

const routes: Routes = [
  {
    path: 'app',
    component: TabsPage,
    children: [
      {
        path: 'groups',
        loadChildren: () =>
          import('../groups/groups.module').then((m) => m.GroupsPageModule),
      },
      {
        path: 'add-view',
        loadChildren: () =>
          import('../add-view/add-view.module').then(
            (m) => m.AddViewPageModule,
          ),
      },
      {
        path: 'expense',
        loadChildren: () =>
          import('../expense/expense.module').then((m) => m.ExpenseModule),
      },
      {
        path: '',
        redirectTo: '/app/groups',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: '',
    redirectTo: '/app/groups',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
})
export class TabsPageRoutingModule {}
