import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth/auth.service';
import { UserService } from '../_services/user/user.service';
import { BehaviorSubject, catchError, delay, of, tap } from 'rxjs';

@Component({
  selector: 'app-user',
  templateUrl: './user.page.html',
  styleUrls: ['./user.page.scss'],
})
export class UserPage implements OnDestroy {
  user$ = this.userService.getYourself().pipe(
    tap(() => this.loading$.next(false)),
    catchError(() => {
      this.loading$.next(false);
      setTimeout(() => {
        this.authService.logout();
      });
      return of(null);
    }),
  );

  loading$ = new BehaviorSubject<boolean>(true);
  constructor(
    protected readonly authService: AuthService,
    private readonly userService: UserService,
  ) {}

  ionViewWillLeave() {
    this.loading$.next(false);
  }

  ngOnDestroy(): void {
    this.loading$.complete();
  }
}
