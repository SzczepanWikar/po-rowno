import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AlertButton } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { delay, Subject, takeUntil } from 'rxjs';
import { DELAY } from 'src/app/_common/constants';
import { Currency } from 'src/app/_common/enums/currency.enum';
import { getCurrencySymbol } from 'src/app/_common/helpers/get-currency-symbol';
import { CreateGroupDto } from 'src/app/_services/group/dto/create-group.dto';
import { GroupService } from 'src/app/_services/group/group.service';

@Component({
  selector: 'app-add-group',
  templateUrl: './add-group.page.html',
  styleUrls: ['./add-group.page.scss'],
})
export class AddGroupPage implements OnInit, OnDestroy {
  protected saving = false;
  protected currencies = new Map([
    [Currency.Dollar, getCurrencySymbol(Currency.Dollar)],
    [Currency.Euro, getCurrencySymbol(Currency.Euro)],
    [Currency.PolishZloty, getCurrencySymbol(Currency.PolishZloty)],
  ]);
  protected form = this.formBuilder.group({
    name: [
      '',
      [Validators.required, Validators.minLength(1), Validators.maxLength(50)],
    ],
    description: [
      '',
      [Validators.required, Validators.minLength(1), Validators.maxLength(400)],
    ],
    currency: new FormControl<Currency | undefined>(undefined, {
      validators: [Validators.required],
    }),
  });

  protected showErrorAlert = false;
  protected errorAlertButtons: ReadonlyArray<AlertButton> = [];

  #destroy$: Subject<void> = new Subject();

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly groupService: GroupService,
    private readonly router: Router,
    private readonly translate: TranslateService,
  ) {}

  ngOnInit() {
    this.errorAlertButtons = [
      {
        role: 'cancel',
        text: this.translate.instant('CLOSE'),
      },
    ];
  }

  onSubmit($event: SubmitEvent): void {
    if (!this.form.valid || !this.form.touched) {
      return;
    }

    this.saving = true;

    const { name, description, currency } = this.form.value;
    const dto: CreateGroupDto = {
      name: name!,
      description: description!,
      currency: +currency!,
    };

    this.groupService
      .create(dto)
      .pipe(takeUntil(this.#destroy$), delay(DELAY))
      .subscribe({
        next: () => {
          this.router.navigate(['app/groups']);
        },
        error: () => {
          this.showErrorAlert = true;
        },
      });
  }

  hideErrorAlert() {
    this.showErrorAlert = false;
  }

  ngOnDestroy(): void {
    this.#destroy$.next();
    this.#destroy$.complete();
  }
}
