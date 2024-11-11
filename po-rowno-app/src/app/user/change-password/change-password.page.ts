import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AlertButton } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject, takeUntil, tap } from 'rxjs';
import { matchFieldsValidator } from 'src/app/_common/validators/match-field.validator';
import { passwordValidator } from 'src/app/_common/validators/password.validator';
import { AuthService } from 'src/app/_services/auth/auth.service';
import { ChangePasswordDto } from 'src/app/_services/auth/dto/change-password.dto';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.page.html',
  styleUrls: ['./change-password.page.scss'],
})
export class ChangePasswordPage implements OnInit, OnDestroy {
  protected object = Object;

  protected form = this.formBuilder.group(
    {
      password: ['', Validators.required],
      newPassword: ['', [Validators.required, passwordValidator()]],
      reapetedPassword: ['', [Validators.required]],
    },
    {
      validators: [matchFieldsValidator('newPassword', 'reapetedPassword')],
    },
  );

  protected showAlert = false;
  protected alertTitle = this.translate.instant('VALIDATION.ERROR_OCCURED');
  protected alertButtons: ReadonlyArray<AlertButton> = [];
  protected alertContent = this.translate.instant('VALIDATION.ERROR_OCCURED');

  protected saving = false;

  #success = false;

  #destroy$ = new Subject<void>();

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly translate: TranslateService,
  ) {}

  ngOnInit() {
    this.alertButtons = [
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

    const { newPassword, password } = this.form.value;

    const dto: ChangePasswordDto = {
      newPassword: newPassword!,
      oldPassword: password!,
    };

    this.authService
      .changePassword(dto)
      .pipe(
        takeUntil(this.#destroy$),
        tap(() => {
          this.saving = false;
        }),
      )
      .subscribe({
        next: () => {
          this.showAlertOnRes(true);
        },
        error: () => {
          this.showAlertOnRes(false);
        },
      });
  }

  showAlertOnRes(success: boolean) {
    this.#success = success;
    if (success) {
      this.alertTitle = this.translate.instant('SUCCESS');
      this.alertContent = this.translate.instant('SUCCESS');
    } else {
      this.alertContent = this.translate.instant('VALIDATION.ERROR_OCCURED');
      this.alertTitle = this.translate.instant('VALIDATION.ERROR_OCCURED');
    }

    this.showAlert = true;
  }

  hideAlert() {
    this.showAlert = false;
    if (this.#success) {
      this.form.reset();
      this.authService.logout();
    } else {
      this.alertContent = this.translate.instant('VALIDATION.ERROR_OCCURED');
    }
    this.saving = false;
  }

  ngOnDestroy(): void {
    this.#destroy$.next();
    this.#destroy$.complete();
  }
}
