import { Expense } from './expense';
import { User } from './user';

export interface Deptor {
  id: string;
  expenseId: string;
  amount: number;
  userId: string;
  expense?: Expense;
  user?: User;
}
