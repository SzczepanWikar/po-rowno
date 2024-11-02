import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Group } from 'src/app/_common/models/group';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class GroupService {
  private readonly url = environment.apiUrl + 'Group';
  constructor(private readonly http: HttpClient) {}

  get(): Observable<Group[]> {
    return this.http.get<Group[]>(this.url);
  }

  getOne(id: string): Observable<Group> {
    return this.http.get<Group>(`${this.url}/${id}`);
  }

  refreshJoinCode(id: string, validTo: Date): Observable<Object> {
    return this.http.patch(`${this.url}/${id}/join-code`, {
      validTo,
    });
  }
}
