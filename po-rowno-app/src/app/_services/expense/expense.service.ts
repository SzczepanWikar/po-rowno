import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ExportQuery } from './dto/expense.query';
import { Expense } from 'src/app/_common/models/expense';
import { AddExpenseDto } from './dto/add-expense.dto';
import { Observable } from 'rxjs';
import { AddExpenseWithPaymentDto } from './dto/add-expense-with-payment.dto';
import { PaymentCreatedDto } from './dto/payment-created.dto';
import { PaymentCapturedDto } from './dto/payment-captured.dto';

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

  getOne(id: string) {
    return this.http.get<Expense>(`${this.url}/${id}`);
  }

  create(dto: AddExpenseDto): Observable<string> {
    return this.http.post<string>(this.url, dto);
  }

  pay(dto: AddExpenseWithPaymentDto): Observable<PaymentCreatedDto> {
    return this.http.post<PaymentCreatedDto>(`${this.url}/payment`, dto);
  }

  capture(orderId: string): Observable<PaymentCapturedDto> {
    return this.http.patch<PaymentCapturedDto>(
      `${this.url}/payment/${orderId}/capture`,
      {},
    );
  }
}
