import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AddViewPage } from './add-view.page';

const routes: Routes = [
  {
    path: '',
    component: AddViewPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AddViewPageRoutingModule {}
