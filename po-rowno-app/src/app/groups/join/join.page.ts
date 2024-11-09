import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AlertButton } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject, takeUntil } from 'rxjs';
import { GroupService } from 'src/app/_services/group/group.service';

@Component({
  selector: 'app-join',
  templateUrl: './join.page.html',
  styleUrls: ['./join.page.scss'],
})
export class JoinPage implements OnInit, OnDestroy {
  protected form = this.formBuilder.group({
    code: ['', [Validators.required]],
  });
  protected saving = false;

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

  onSubmit() {
    if (!this.form.valid || !this.form.touched) {
      return;
    }
    const code = this.form.value.code ?? '';

    this.groupService
      .join(code)
      .pipe(takeUntil(this.#destroy$))
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
