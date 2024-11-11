import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth/auth.service';
import { Subject, takeUntil, tap } from 'rxjs';
import { passwordValidator } from '../_common/validators/password.validator';
import { matchFieldsValidator } from '../_common/validators/match-field.validator';
import { ResetPasswordDto } from '../_services/auth/dto/reset-password.dto';
import { HttpErrorResponse } from '@angular/common/http';
import { AlertButton } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.page.html',
  styleUrls: ['./reset-password.page.scss'],
})
export class ResetPasswordPage {
  protected step: Step = 1;
  protected saving = false;
  #email = '';
  #destroy$ = new Subject<void>();
  protected readonly object = Object;

  protected requestForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
  });

  protected resetPasswordForm = this.formBuilder.group(
    {
      password: ['', [Validators.required, passwordValidator()]],
      reapetedPassword: ['', [Validators.required]],
      code: ['', [Validators.required]],
    },
    {
      validators: [matchFieldsValidator('password', 'reapetedPassword')],
    },
  );

  protected showErrorAlert = false;
  protected errorAlertTitle = this.translate.instant(
    'VALIDATION.ERROR_OCCURED',
  );
  protected errorAlertButtons: ReadonlyArray<AlertButton> = [
    {
      role: 'cancel',
      text: this.translate.instant('CLOSE'),
    },
  ];
  protected errorAlertContent = '';

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly translate: TranslateService,
  ) {}

  onRequestSubmit() {
    if (!this.requestForm.valid || !this.requestForm.touched) {
      return;
    }
    this.saving = true;
    this.#email = this.requestForm.value.email ?? '';
    this.authService
      .requestResetPassword(this.#email)
      .pipe(takeUntil(this.#destroy$))
      .subscribe({
        next: () => this.#nextStep(2),
        error: () => this.#nextStep(2),
      });
  }

  onResetSubmit() {
    if (!this.resetPasswordForm.valid || !this.resetPasswordForm.touched) {
      return;
    }

    this.saving = true;

    const { password, reapetedPassword, code } = this.resetPasswordForm.value;
    const dto: ResetPasswordDto = {
      email: this.#email,
      password: password!,
      confirmPassword: reapetedPassword!,
      code: code!,
    };

    this.authService
      .resetPassword(dto)
      .pipe(takeUntil(this.#destroy$))
      .subscribe({
        next: () => this.#nextStep(3),
        error: (error: HttpErrorResponse) => {
          if (
            error.status === 404 ||
            (error.status === 400 && error.error === 'Invalid code.')
          ) {
            this.showErrorAlertOnRes('VALIDATION.INVALID_CODE');
          } else {
            this.showErrorAlertOnRes('VALIDATION.ERROR_OCCURED');
          }
          this.saving = false;
        },
      });
  }

  showErrorAlertOnRes(contentCode: string) {
    this.errorAlertContent = this.translate.instant(contentCode);
    this.showErrorAlert = true;
  }

  hideErrorAlert() {
    this.showErrorAlert = false;
    this.errorAlertContent = '';
  }

  #nextStep(step: Step) {
    this.saving = false;
    this.step = step;
  }
}

type Step = 1 | 2 | 3;
