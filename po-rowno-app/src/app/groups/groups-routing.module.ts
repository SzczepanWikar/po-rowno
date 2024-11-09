import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { GroupsPage } from './groups.page';

const routes: Routes = [
  {
    path: '',
    component: GroupsPage,
  },
  {
    path: 'add',
    loadChildren: () =>
      import('./add-group/add-group.module').then((m) => m.AddGroupPageModule),
  },
  {
    path: 'join',
    loadChildren: () =>
      import('./join/join.module').then((m) => m.JoinPageModule),
  },
  {
    path: ':id',
    loadChildren: () =>
      import('./group-details/group-details.module').then(
        (m) => m.GroupDetailsPageModule,
      ),
  },
  {
    path: ':id/edit',
    loadChildren: () =>
      import('./edit-group/edit-group.module').then(
        (m) => m.EditGroupPageModule,
      ),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GroupsPageRoutingModule {}
