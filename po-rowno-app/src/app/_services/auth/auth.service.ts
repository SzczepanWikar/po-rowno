import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SignInDto } from './dto/sign-in.dto';
import { HttpClient } from '@angular/common/http';
import { AppSignInResult } from './dto/sign-in-result';
import { Observable, tap } from 'rxjs';
import { ACCESS_TOKEN, REFRESH_TOKEN, USER_ID } from '../../_common/constants';
import { SignUpDto } from './dto/sign-up.dto';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly url = environment.apiUrl + 'User/';
  constructor(private readonly http: HttpClient) {}

  signIn(dto: SignInDto): Observable<AppSignInResult> {
    return this.http
      .post<AppSignInResult>(this.url + 'sign-in', dto)
      .pipe(tap(this.saveTokensInStorage));
  }

  refresh(): Observable<AppSignInResult> {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN);

    return this.http
      .post<AppSignInResult>(this.url + 'refresh', {
        refreshToken,
      })
      .pipe(tap(this.saveTokensInStorage));
  }

  signUp(dto: SignUpDto): Observable<string> {
    return this.http.post<string>(this.url + 'sign-up', dto);
  }

  private saveTokensInStorage(result: AppSignInResult): void {
    localStorage.setItem(ACCESS_TOKEN, result.accessToken);
    localStorage.setItem(REFRESH_TOKEN, result.refreshToken);
    localStorage.setItem(USER_ID, result.id);
  }
}
