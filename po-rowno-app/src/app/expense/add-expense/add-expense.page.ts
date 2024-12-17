import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AlertButton } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { set } from 'date-fns';
import {
  BehaviorSubject,
  delay,
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
import { AddExpenseDto } from 'src/app/_services/expense/dto/add-expense.dto';
import { ExpenseDeptorDto } from 'src/app/_services/expense/dto/expense-deptor.dto';
import { ExpenseService } from 'src/app/_services/expense/expense.service';
import { GroupService } from 'src/app/_services/group/group.service';

@Component({
  selector: 'app-add-expense',
  templateUrl: './add-expense.page.html',
  styleUrls: ['./add-expense.page.scss'],
})
export class AddExpensePage implements OnInit, OnDestroy {
  protected saving = false;
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
    amount: ['', [Validators.required, Validators.min(0.02)]],
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

  protected showErrorAlert = false;
  protected errorAlertButtons: ReadonlyArray<AlertButton> = [];

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    private readonly translate: TranslateService,
    private readonly groupService: GroupService,
    private readonly expenseService: ExpenseService,
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

  ngOnInit() {
    this.errorAlertButtons = [
      {
        role: 'cancel',
        text: this.translate.instant('CLOSE'),
      },
    ];
  }

  onGroupSet() {
    const id = this.form.value.groupId;

    if (!id?.length) {
      return;
    }
    this.loading$.next(true);
    this.#groupId$.next(id);
  }

  restrictDecimalPlaces(event: CustomEvent) {
    const inputValue = event.detail.value;

    if (inputValue.includes('.') || inputValue.includes(',')) {
      const [integerPart, decimalPart] = inputValue.split(/[.,]/);
      decimalPart.concat('0');

      this.form
        .get('amount')
        ?.setValue(`${integerPart}.${decimalPart.slice(0, 2)}`);
    }
  }

  onSubmit($event: SubmitEvent) {
    if (!this.form.valid || !this.form.touched) {
      return;
    }

    this.saving = true;
    this.loading$.next(true);

    const { name, amount, groupId, userIds, type } = this.form.value;
    const deptors = this.#calcDeptors(userIds!, +amount!);
    const dto: AddExpenseDto = {
      name: name!,
      amount: +amount!,
      groupId: groupId!,
      currency: this.#currentCurrency!,
      type: +type!,
      deptors,
    };

    this.expenseService
      .create(dto)
      .pipe(
        tap(() => {
          this.saving = false;
          this.loading$.next(false);
        }),
        delay(0),
      )
      .subscribe({
        next: () => {
          this.router.navigate([`app/groups/${groupId}`]);
        },
        error: () => {
          this.showErrorAlert = true;
        },
      });
  }

  ionViewWillLeave() {
    this.loading$.next(false);
  }

  ngOnDestroy(): void {
    this.loading$.complete();
    this.#destroy$.next();
    this.#destroy$.complete();
  }

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

  #calcDeptors(userIds: string[], amount: number): ExpenseDeptorDto[] {
    const rawDept = amount / (userIds.length + 1);
    const dept = Math.floor(rawDept * 100) / 100;

    return userIds.map((e) => ({
      amount: dept,
      userId: e,
    }));
  }

  hideErrorAlert() {
    this.showErrorAlert = false;
  }
}
