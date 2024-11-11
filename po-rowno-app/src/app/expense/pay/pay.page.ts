import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AlertButton } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import {
  BehaviorSubject,
  catchError,
  delay,
  interval,
  Observable,
  of,
  Subject,
  switchMap,
  takeUntil,
  takeWhile,
  tap,
  throwError,
} from 'rxjs';
import { USER_ID } from 'src/app/_common/constants';
import { Currency } from 'src/app/_common/enums/currency.enum';
import { ExpenseType } from 'src/app/_common/enums/expense-type.enum';
import { UserGroupStatus } from 'src/app/_common/enums/user-group-status.enum';
import { getExpenseTypeTranslateKey } from 'src/app/_common/helpers/get-expense-translate-key';
import { Group } from 'src/app/_common/models/group';
import { User } from 'src/app/_common/models/user';
import { AddExpenseWithPaymentDto } from 'src/app/_services/expense/dto/add-expense-with-payment.dto';
import { ExpenseService } from 'src/app/_services/expense/expense.service';
import { GroupService } from 'src/app/_services/group/group.service';

@Component({
  selector: 'app-pay',
  templateUrl: './pay.page.html',
  styleUrls: ['./pay.page.scss'],
})
export class PayPage implements OnInit, OnDestroy {
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
    amount: ['', [Validators.required, Validators.min(0.01)]],
    groupId: new FormControl<string>(
      { value: '', disabled: true },
      { validators: [Validators.required] },
    ),
    userId: new FormControl<string>(
      {
        value: '',
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
          this.form.controls.userId.disable();
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

    const { amount, userId, groupId } = this.form.value;

    const dto: AddExpenseWithPaymentDto = {
      amount: +amount!,
      currency: this.#currentCurrency!,
      groupId: groupId!,
      receiverId: userId!,
    };

    let orderId = '';
    this.expenseService
      .pay(dto)
      .pipe(
        takeUntil(this.#destroy$),
        tap((e) => (orderId = e.orderId)),
        tap((e) => window.open(e.links.find((f) => f.rel === 'approve')?.href)),
        switchMap((e) =>
          interval(2000).pipe(
            switchMap(() => this.expenseService.getOne(e.id)),
            takeWhile((f) => f.paymentStatus === 'CREATED', true),
            switchMap((f) => {
              if (f.paymentStatus === 'APPROVED') {
                return this.expenseService.capture(orderId);
              } else {
                throwError(() => new Error());
              }
              return [];
            }),
            catchError(() => {
              this.showErrorAlert = true;
              return [];
            }),
          ),
        ),
        tap(() => {
          this.loading$.next(false);
        }),
        delay(0),
      )
      .subscribe({
        next: () => {
          this.router.navigate([`app/groups/${groupId}`]);
        },
        error: () => {},
      });
  }

  ngOnDestroy(): void {
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
      this.form.controls.userId.enable();
    }

    this.loading$.next(false);
  }

  hideErrorAlert() {
    this.showErrorAlert = false;
    setTimeout(() => {
      this.router.navigate([`app/groups/${this.form.value.groupId}`]);
    });
  }
}
