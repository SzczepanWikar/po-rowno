import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from 'src/app/_common/models/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly url = environment.apiUrl + 'User';
  constructor(private readonly http: HttpClient) {}

  getYourself(): Observable<User> {
    return this.http.get<User>(`${this.url}/yourself`);
  }
}
