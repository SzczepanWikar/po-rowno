import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SignInDto } from '../sign-in/dto/sign-in.dto';
import { HttpClient } from '@angular/common/http';
import { AppSignInResult } from '../sign-in/dto/sign-in-result';
import { Observable, tap } from 'rxjs';

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
    localStorage.setItem('auth_token', result.accessToken);
    localStorage.setItem('refresh_token', result.refreshToken);
  }
}
