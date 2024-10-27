import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SignUpPage } from './sign-up.page';

const routes: Routes = [
  {
    path: '',
    component: SignUpPage,
  },
  {
    path: 'success',
    loadChildren: () =>
      import('./sign-up-success/sign-up-success.module').then(
        (m) => m.SignUpSuccessPageModule,
      ),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SignUpPageRoutingModule {}
