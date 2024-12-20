import { Currency } from '../enums/currency.enum';
import { ExpenseType } from '../enums/expense-type.enum';
import { Deptor } from './deptor';
import { Group } from './group';
import { User } from './user';

export type PaymentStatus = 'CREATED' | 'APPROVED' | 'CANCELED' | 'COMPLETED';

export interface Expense {
  id: string;
  name: string;
  amount: number;
  currency: Currency;
  groupId: string;
  type: ExpenseType;
  payerId?: string;
  paymentStatus?: PaymentStatus;
  group?: Group;
  payer?: User;
  deptors?: Deptor[];
}
