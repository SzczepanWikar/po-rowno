import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class GroupService {
  private readonly url = environment.apiUrl + 'Group';
  constructor(private readonly http: HttpClient) {}
}
