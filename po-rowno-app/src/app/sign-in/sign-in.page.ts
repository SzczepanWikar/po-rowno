import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth/auth.service';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.page.html',
  styleUrls: ['./sign-in.page.scss'],
})
export class SignInPage implements OnDestroy {
  protected authForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  private destroy$ = new Subject<void>();

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {}

  onSubmit($event: SubmitEvent) {
    this.authService
      .signIn({
        email: this.authForm.value.email!,
        password: this.authForm.value.password!,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: async () => {
          await this.router.navigate(['app']);
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
