import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import {
  BehaviorSubject,
  Observable,
  of,
  Subject,
  switchMap,
  takeUntil,
  tap,
} from 'rxjs';
import { USER_ID } from 'src/app/_common/constants';
import { Currency } from 'src/app/_common/enums/currency.enum';
import { ExpenseType } from 'src/app/_common/enums/expense-type.enum';
import { UserGroupStatus } from 'src/app/_common/enums/user-group-status.enum';
import { getExpenseTypeTranslateKey } from 'src/app/_common/helpers/get-expense-translate-key';
import { Group } from 'src/app/_common/models/group';
import { User } from 'src/app/_common/models/user';
import { GroupService } from 'src/app/_services/group/group.service';

@Component({
  selector: 'app-add-expense',
  templateUrl: './add-expense.page.html',
  styleUrls: ['./add-expense.page.scss'],
})
export class AddExpensePage implements OnInit {
  protected saving = false;
  protected readonly locale: string = navigator.language;
  protected readonly defaultType = ExpenseType.Cost;

  protected readonly expenseTypes = new Map([
    [ExpenseType.Cost, getExpenseTypeTranslateKey(ExpenseType.Cost)],
    [
      ExpenseType.Settlement,
      getExpenseTypeTranslateKey(ExpenseType.Settlement),
    ],
  ]);

  protected form = this.formBuilder.group({
    name: [
      '',
      [Validators.required, Validators.minLength(1), Validators.maxLength(50)],
    ],
    amount: [0, [Validators.required, Validators.min(0.02)]],
    type: new FormControl<ExpenseType | undefined>(undefined, {
      validators: [Validators.required],
    }),
    groupId: new FormControl<string>(
      { value: '', disabled: true },
      { validators: [Validators.required] },
    ),
    userIds: new FormControl<string[]>(
      {
        value: [],
        disabled: true,
      },
      {
        validators: [Validators.required],
      },
    ),
  });

  protected groups$: Observable<Group[]> = this.groupService.get().pipe(
    tap(() => {
      this.form.controls.groupId.enable(), this.loading$.next(false);
    }),
  );
  protected users$ = new BehaviorSubject<User[]>([]);
  protected loading$ = new BehaviorSubject<boolean>(true);
  #groupId$ = new Subject<string>();
  #destroy$ = new Subject<void>();

  #downladedGroups = new Map<string, Group>();
  #currentCurrency: Currency | undefined;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    private readonly translate: TranslateService,
    private readonly groupService: GroupService,
  ) {
    this.#groupId$
      .pipe(
        takeUntil(this.#destroy$),
        switchMap((e) => {
          this.form.controls.userIds.disable();
          if (this.#downladedGroups.has(e)) {
            return of(this.#downladedGroups.get(e)!);
          }

          return this.groupService.getOne(e);
        }),
        tap((e) => {
          if (!this.#downladedGroups.has(e.id)) {
            this.#downladedGroups.set(e.id, e);
          }
        }),
        tap((e) => {
          this.#setGroupIdData(e);
        }),
      )
      .subscribe();
  }
  ngOnInit() {}

  onGroupSet() {
    const id = this.form.value.groupId;

    if (!id?.length) {
      return;
    }
    this.loading$.next(true);
    this.#groupId$.next(id);
  }

  onSubmit($event: SubmitEvent) {}

  #setGroupIdData(e: Group) {
    const users = e.userGroups?.flatMap((u) =>
      u.status === UserGroupStatus.Active &&
      u.userId !== localStorage.getItem(USER_ID)
        ? (u.user ?? [])
        : [],
    );

    this.#currentCurrency = e.currency;
    this.users$.next(users ?? []);

    if (users?.length) {
      this.form.controls.userIds.enable();
    }

    this.loading$.next(false);
  }
}
