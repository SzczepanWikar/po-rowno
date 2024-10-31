import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { GroupsPage } from './groups.page';

const routes: Routes = [
  {
    path: '',
    component: GroupsPage,
  },
  {
    path: ':id',
    loadChildren: () =>
      import('./group-details/group-details.module').then(
        (m) => m.GroupDetailsPageModule,
      ),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GroupsPageRoutingModule {}
