import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SignInDto } from '../sign-in/dto/sign-in.dto';
import { HttpClient } from '@angular/common/http';
import { AppSignInResult } from '../sign-in/dto/sign-in-result';
import { Observable, tap } from 'rxjs';
import { ACCESS_TOKEN, REFRESH_TOKEN } from '../_common/constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly url = environment.apiUrl;
  constructor(private readonly http: HttpClient) {}

  signIn(dto: SignInDto): Observable<AppSignInResult> {
    return this.http
      .post<AppSignInResult>(this.url + 'User/sign-in', dto)
      .pipe(tap(this.saveTokensInStorage));
  }

  private saveTokensInStorage(result: AppSignInResult): void {
    localStorage.setItem(ACCESS_TOKEN, result.accessToken);
    localStorage.setItem(REFRESH_TOKEN, result.refreshToken);
  }
}
