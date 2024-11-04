import { Component, OnInit } from '@angular/core';
import { InfiniteScrollCustomEvent } from '@ionic/angular';
import { BehaviorSubject, Observable, switchMap, tap } from 'rxjs';
import { ExpenseService } from 'src/app/_services/expense/expense.service';
import { GroupDetailsService } from '../group-details.service';
import { Expense } from 'src/app/_common/models/expense';

@Component({
  selector: 'app-group-expenses',
  templateUrl: './group-expenses.component.html',
  styleUrls: ['./group-expenses.component.scss'],
})
export class GroupExpensesComponent implements OnInit {
  currentPage$ = new BehaviorSubject<number>(1);
  #currentInfiniteEvent?: InfiniteScrollCustomEvent;

  currentPageData$: Observable<Expense[]> = this.currentPage$.pipe(
    switchMap((p) =>
      this.expenseService.getAll({
        GroupId: this.groupService.groupId,
        Take: 20,
        Page: p,
        Ascending: false,
      }),
    ),
    tap(() => {
      if (this.#currentInfiniteEvent) {
        this.#currentInfiniteEvent.target.complete();
        this.#currentInfiniteEvent = undefined;
      }
    }),
  );

  constructor(
    private readonly expenseService: ExpenseService,
    protected readonly groupService: GroupDetailsService,
  ) {}

  ngOnInit() {}

  protected loadData(event: Event) {
    this.#currentInfiniteEvent = event as InfiniteScrollCustomEvent;
    this.currentPage$.next(this.currentPage$.value + 1);
  }
}
