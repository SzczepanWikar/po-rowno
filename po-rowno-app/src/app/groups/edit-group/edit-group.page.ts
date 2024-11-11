import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertButton } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';
import { UserGroupStatus } from 'src/app/_common/enums/user-group-status.enum';
import { Group } from 'src/app/_common/models/group';
import { User } from 'src/app/_common/models/user';
import { UserGroup } from 'src/app/_common/models/user-group';
import { UpdateGroupDto } from 'src/app/_services/group/dto/update-group.dto';
import { GroupService } from 'src/app/_services/group/group.service';

@Component({
  selector: 'app-edit-group',
  templateUrl: './edit-group.page.html',
  styleUrls: ['./edit-group.page.scss'],
})
export class EditGroupPage implements OnInit, OnDestroy {
  protected loading$: Subject<boolean> = new BehaviorSubject(true);
  #destroy$: Subject<void> = new Subject();

  protected group?: Group;
  protected users: User[] = [];

  protected form = this.formBuilder.group({
    name: ['', [Validators.minLength(1), Validators.maxLength(50)]],
    description: ['', [Validators.minLength(1), Validators.maxLength(400)]],
    ownerId: new FormControl<string>(''),
  });

  protected showErrorAlert = false;
  protected errorAlertButtons: ReadonlyArray<AlertButton> = [];

  constructor(
    private readonly formBuilder: FormBuilder,
    protected readonly groupService: GroupService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly translate: TranslateService,
  ) {}

  ngOnInit() {
    this.#setupErrorButtons();
    this.#getGroup();
  }

  ngOnDestroy(): void {
    this.loading$.next(false);
    this.loading$.complete();
    this.#destroy$.next();
    this.#destroy$.complete();
  }

  initForm(group: Group) {
    const { name, description, ownerId } = group;
    this.form.setValue({ name, description, ownerId });
  }

  onSubmit($event: SubmitEvent): void {
    if (!this.form.valid || !this.form.touched) {
      return;
    }

    this.loading$.next(true);

    const { name, description, ownerId } = this.form.value;

    const dto: UpdateGroupDto = {
      name,
      description,
      ownerId,
    };

    this.groupService
      .update(this.group?.id ?? '', dto)
      .pipe(takeUntil(this.#destroy$))
      .subscribe({
        next: () => {
          this.loading$.next(false);
          setTimeout(() => {
            this.router.navigate([`app/groups/${this.group?.id}`]);
          });
        },
        error: () => {
          this.loading$.next(false);
          this.showErrorAlert = true;
        },
      });
  }

  hideErrorAlert() {
    this.showErrorAlert = false;
  }

  #setupErrorButtons() {
    this.errorAlertButtons = [
      {
        role: 'cancel',
        text: this.translate.instant('CLOSE'),
      },
    ];
  }

  #getGroup() {
    this.route.params.pipe(takeUntil(this.#destroy$)).subscribe((e) => {
      this.groupService
        .getOne(e['id'])
        .pipe(takeUntil(this.#destroy$))
        .subscribe({
          next: (e) => {
            this.group = e;
            this.users =
              e.userGroups?.flatMap((e) =>
                e.status === UserGroupStatus.Active ? (e.user ?? []) : [],
              ) ?? [];
            this.initForm(e);

            this.loading$.next(false);
          },
          error: () => {
            this.router.navigate(['app/groups']);
          },
        });
    });
  }
}
