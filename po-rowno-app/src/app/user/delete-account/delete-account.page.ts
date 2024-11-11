import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth/auth.service';

@Component({
  selector: 'app-delete-account',
  templateUrl: './delete-account.page.html',
  styleUrls: ['./delete-account.page.scss'],
})
export class DeleteAccountPage {
  protected form = this.formBuilder.group({
    password: ['', Validators.required],
  });

  protected saving = false;

  constructor(
    private readonly authService: AuthService,
    private readonly formBuilder: FormBuilder,
  ) {}

  onSubmit() {
    if (!(this.form.valid && this.form.touched)) {
      return;
    }

    this.saving = true;

    const { password } = this.form.value;

    this.authService.delete(password!).subscribe({
      next: () => {
        this.saving = false;
        this.authService.logout();
      },
      error: () => {
        this.saving = false;
      },
    });
  }
}
