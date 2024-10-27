import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SignUpSuccessPage } from './sign-up-success.page';

const routes: Routes = [
  {
    path: '',
    component: SignUpSuccessPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SignUpSuccessPageRoutingModule {}
