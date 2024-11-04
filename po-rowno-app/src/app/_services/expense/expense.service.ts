import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ExportQuery } from './dto/expense.query';
import { Expense } from 'src/app/_common/models/expense';

@Injectable({
  providedIn: 'root',
})
export class ExpenseService {
  private readonly url: string = environment.apiUrl + 'Expense';

  constructor(private readonly http: HttpClient) {}

  getAll(query: ExportQuery) {
    return this.http.get<Expense[]>(this.url, {
      params: { ...query },
    });
  }
}
