import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Group } from 'src/app/_common/models/group';
import { environment } from 'src/environments/environment';
import { CreateGroupDto } from './dto/create-group.dto';
import { UpdateGroupDto } from './dto/update-group.dto';

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

  create(dto: CreateGroupDto): Observable<string> {
    return this.http.post<string>(this.url, dto);
  }

  update(id: string, dto: UpdateGroupDto): Observable<void> {
    return this.http.patch<void>(`${this.url}/${id}`, dto);
  }

  refreshJoinCode(id: string, validTo: Date): Observable<Object> {
    return this.http.patch(`${this.url}/${id}/join-code`, {
      validTo,
    });
  }

  join(code: string): Observable<void> {
    return this.http.patch<void>(`${this.url}/join`, { code });
  }

  banUser(id: string, userId: string): Observable<unknown> {
    return this.http.patch(`${this.url}/${id}/ban-user`, {
      id: userId,
    });
  }

  unbanUser(id: string, userId: string): Observable<unknown> {
    return this.http.patch(`${this.url}/${id}/unban-user`, {
      id: userId,
    });
  }

  leave(id: string): Observable<unknown> {
    return this.http.patch(`${this.url}/${id}/leave`, {});
  }
}
