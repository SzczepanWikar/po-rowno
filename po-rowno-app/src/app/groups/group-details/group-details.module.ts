import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { GroupDetailsPageRoutingModule } from './group-details-routing.module';

import { GroupDetailsPage } from './group-details.page';
import { GroupDetailsService } from './group-details.service';
import { GroupUsersComponent } from './group-users/group-users.component';
import { GroupExpensesComponent } from './group-expenses/group-expenses.component';
import { GroupCodeComponent } from './group-code/group-code.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    GroupDetailsPageRoutingModule,
    TranslateModule,
  ],
  providers: [GroupDetailsService],
  declarations: [
    GroupDetailsPage,
    GroupUsersComponent,
    GroupExpensesComponent,
    GroupCodeComponent,
  ],
})
export class GroupDetailsPageModule {}
