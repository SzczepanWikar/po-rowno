import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth/auth.service';
import { passwordValidator } from '../_common/validators/password.validator';
import { matchFieldsValidator } from '../_common/validators/match-field.validator';
import { TranslateService } from '@ngx-translate/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject, takeUntil } from 'rxjs';
import { AlertButton } from '@ionic/angular';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.page.html',
  styleUrls: ['./sign-up.page.scss'],
})
export class SignUpPage implements OnInit, OnDestroy {
  protected object = Object;
  protected signUpForm = this.formBuilder.group(
    {
      username: ['', [Validators.required, Validators.minLength(1)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, passwordValidator()]],
      reapetedPassword: ['', [Validators.required]],
    },
    {
      validators: [matchFieldsValidator('password', 'reapetedPassword')],
    },
  );
  protected showErrorAlert = false;
  protected errorAlertTitle = '';
  protected errorAlertButtons: ReadonlyArray<AlertButton> = [];
  protected errorAlertContent = '';
  protected saving = false;

  private destroy$ = new Subject<void>();

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly translate: TranslateService,
  ) {}

  ngOnInit(): void {
    this.errorAlertTitle = this.translate.instant('VALIDATION.ERROR_OCCURED');
    this.errorAlertContent = this.translate.instant('VALIDATION.ERROR_OCCURED');
    this.errorAlertButtons = [
      {
        role: 'cancel',
        text: this.translate.instant('CLOSE'),
      },
    ];
  }

  onSubmit($event: SubmitEvent): void {
    if (!this.signUpForm.valid || !this.signUpForm.touched) {
      return;
    }

    this.saving = true;

    this.authService
      .signUp({
        username: this.signUpForm.value!.username!,
        email: this.signUpForm.value!.email!,
        password: this.signUpForm.value!.password!,
        reapetedPassword: this.signUpForm.value!.reapetedPassword!,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.signUpForm.reset();
          this.router.navigate(['sign-up/success']);
        },
        error: (error: HttpErrorResponse) => {
          if (error.status === 409) {
            this.openErrorAlert('AUTH.EMAIL_ALREADY_IN_USE');
          }

          this.saving = false;
        },
      });
  }

  openErrorAlert(content: string): void {
    this.errorAlertContent = this.translate.instant(content);
    this.showErrorAlert = true;
  }

  hideErrorAlert(): void {
    this.showErrorAlert = false;
    this.errorAlertContent = this.translate.instant('VALIDATION.ERROR_OCCURED');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
