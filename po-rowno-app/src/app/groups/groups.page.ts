import { Component, OnInit } from '@angular/core';
import { GroupService } from '../_services/group/group.service';
import { BehaviorSubject, map, Observable, Subject } from 'rxjs';
import { Group } from '../_common/models/group';
import { getCurrencySymbol } from '../_common/helpers/get-currency-symbol';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.page.html',
  styleUrls: ['./groups.page.scss'],
})
export class GroupsPage implements OnInit {
  protected groups$: Observable<Group[]>;
  protected loading$: Subject<boolean> = new BehaviorSubject(true);
  protected getCurrencySymbolDelegate = getCurrencySymbol;

  constructor(private readonly groupService: GroupService) {
    this.groups$ = this.groupService.get().pipe(
      map((e) => {
        this.loading$.next(false);
        return e;
      }),
    );
  }

  ngOnInit() {}
}