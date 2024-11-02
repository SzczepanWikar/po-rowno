import { Component, OnDestroy, OnInit } from '@angular/core';
import { GroupDetailsService } from '../group-details.service';
import { UserGroupStatus } from 'src/app/_common/enums/user-group-status.enum';
import { UserGroup } from 'src/app/_common/models/user-group';
import { USER_ID } from 'src/app/_common/constants';
import { Subject, takeUntil } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-group-users',
  templateUrl: './group-users.component.html',
  styleUrls: ['./group-users.component.scss'],
})
export class GroupUsersComponent implements OnInit, OnDestroy {
  protected activeUsers: UserGroup[] = [];
  protected bannedUsers: UserGroup[] = [];
  protected userId: string = localStorage.getItem(USER_ID) ?? '';

  private destroy$: Subject<void> = new Subject();

  constructor(
    protected readonly groupService: GroupDetailsService,
    protected readonly router: Router,
  ) {}

  ngOnInit() {
    this.groupUsers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected banUser(id: string) {
    return this.groupService
      .banUser(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.groupUsers();
      });
  }

  protected unbanUser(id: string) {
    return this.groupService
      .unbanUser(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.groupUsers();
      });
  }

  protected leaveGroup() {
    this.groupService
      .leave()
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.router.navigate(['app/Groups']);
      });
  }

  private groupUsers() {
    this.activeUsers = [];
    this.bannedUsers = [];

    for (const user of this.groupService.group?.userGroups ?? []) {
      switch (user.status) {
        case UserGroupStatus.Active:
          this.activeUsers.push(user);
          break;
        case UserGroupStatus.Banned:
          this.bannedUsers.push(user);
          break;
      }
    }
  }
}
