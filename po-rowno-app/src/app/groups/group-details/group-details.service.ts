import { Injectable } from '@angular/core';
import { GroupService } from '../../_services/group/group.service';
import { Group } from 'src/app/_common/models/group';
import { concatMap, delay, EMPTY, map } from 'rxjs';
import { Balance } from 'src/app/_common/models/balance';
import { User } from 'src/app/_common/models/user';
import { getCurrencySymbol } from 'src/app/_common/helpers/get-currency-symbol';
import { UserStatus } from 'src/app/_common/enums/user-status.enum';

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
    return this.groupService.refreshJoinCode(this.groupId, validTo).pipe(
      concatMap(() => {
        return this.getGroup(this.groupId);
      }),
    );
  }

  banUser(id: string) {
    return this.groupService.banUser(this.groupId, id).pipe(
      delay(20),
      concatMap(() => {
        return this.getGroup(this.groupId);
      }),
    );
  }

  unbanUser(id: string) {
    return this.groupService.unbanUser(this.groupId, id).pipe(
      delay(20),
      concatMap(() => {
        return this.getGroup(this.groupId);
      }),
    );
  }

  leave() {
    return this.groupService.leave(this.groupId);
  }

  clearState() {
    this.group = undefined;
    this.balances = [];
    this.groupId = '';
    this.currencySymbol = '';
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
      const deptor = users.get(balance.deptorId);

      res.push({
        balance,
        payer: payer ?? this.createNullUser(balance.payerId),
        deptor: deptor ?? this.createNullUser(balance.payerId),
      });
    }

    return res;
  }

  private createNullUser(id: string): User {
    return {
      id,
      username: 'GROUP.USER_LEAVED',
      email: '',
      status: UserStatus.Inactive,
    };
  }
}
