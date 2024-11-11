import { Component, OnDestroy, OnInit } from '@angular/core';
import { GroupService } from '../_services/group/group.service';
import {
  BehaviorSubject,
  catchError,
  map,
  Observable,
  Subject,
  tap,
  throwError,
} from 'rxjs';
import { Group } from '../_common/models/group';
import { getCurrencySymbol } from '../_common/helpers/get-currency-symbol';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.page.html',
  styleUrls: ['./groups.page.scss'],
})
export class GroupsPage implements OnDestroy {
  protected groups$: Observable<Group[]>;
  protected loading$: Subject<boolean> = new BehaviorSubject(true);
  protected getCurrencySymbolDelegate = getCurrencySymbol;

  constructor(private readonly groupService: GroupService) {
    this.groups$ = this.groupService.get().pipe(
      catchError((err) => {
        this.loading$.next(false);
        return throwError(() => err);
      }),
      tap(() => {
        this.loading$.next(false);
      }),
    );
  }
  ngOnDestroy(): void {
    this.loading$.next(false);
    this.loading$.complete();
  }
}
