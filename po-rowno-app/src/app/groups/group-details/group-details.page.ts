import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';
import { Currency } from 'src/app/_common/enums/currency.enum';
import { getCurrencySymbol } from 'src/app/_common/helpers/get-currency-symbol';
import { Balance } from 'src/app/_common/models/balance';
import { Group } from 'src/app/_common/models/group';
import { User } from 'src/app/_common/models/user';
import { GroupService } from 'src/app/_services/group/group.service';
import { GroupDetailsService } from './group-details.service';

@Component({
  selector: 'app-group-details',
  templateUrl: './group-details.page.html',
  styleUrls: ['./group-details.page.scss'],
})
export class GroupDetailsPage implements OnInit, OnDestroy {
  protected loading$: Subject<boolean> = new BehaviorSubject(true);
  protected destroy$: Subject<void> = new Subject();

  protected group?: Group;
  protected balances: BalanceUser[] = [];
  protected currencySymbol: string = '';

  constructor(
    private readonly groupService: GroupDetailsService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
  ) {}

  ngOnInit() {
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe((e) => {
      console.log(e);
      this.groupService
        .getGroup(e['id'])
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (e) => {
            this.group = e;
            this.currencySymbol = getCurrencySymbol(e.currency);
            this.balances = this.mapBalances(e.balances);
            this.loading$.next(false);
          },
          error: () => {
            this.router.navigate(['app/groups']);
          },
        });
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  mapBalances(balances?: Balance[]): BalanceUser[] {
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

interface BalanceUser {
  balance: Balance;
  payer: User;
  deptor: User;
}