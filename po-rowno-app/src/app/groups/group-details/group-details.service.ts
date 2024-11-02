import { Injectable } from '@angular/core';
import { GroupService } from '../../_services/group/group.service';
import { Group } from 'src/app/_common/models/group';
import { map } from 'rxjs';
import { Balance } from 'src/app/_common/models/balance';
import { User } from 'src/app/_common/models/user';
import { getCurrencySymbol } from 'src/app/_common/helpers/get-currency-symbol';

type BalanceUser = {
  balance: Balance;
  payer: User;
  deptor: User;
};

@Injectable()
export class GroupDetailsService {
  group?: Group;
  groupId: string = '';
  balances: BalanceUser[] = [];
  currencySymbol: string = '';

  constructor(private readonly groupService: GroupService) {}

  getGroup(id: string) {
    this.groupId = id;
    return this.groupService.getOne(id).pipe(
      map((e) => {
        this.group = e;
        this.balances = this.mapBalances(e.balances);
        this.currencySymbol = getCurrencySymbol(e.currency);

        return e;
      }),
    );
  }

  refreshJoinCode(validTo: Date) {
    return this.groupService.refreshJoinCode(this.groupId, validTo).pipe(() => {
      return this.getGroup(this.groupId);
    });
  }

  private mapBalances(balances?: Balance[]): BalanceUser[] {
    if (!balances?.length) {
      return [];
    }

    const res: BalanceUser[] = [];
    const users = new Map(
      this.group?.userGroups?.flatMap((e) =>
        e.user ? [[e.userId, e.user]] : [],
      ),
    );

    for (const balance of balances) {
      if (balance.balance <= 0) {
        continue;
      }

      const payer = users.get(balance.payerId);
      if (!payer) {
        continue;
      }

      const deptor = users.get(balance.deptorId);
      if (!deptor) {
        continue;
      }

      res.push({
        balance,
        payer,
        deptor,
      });
    }

    return res;
  }
}
