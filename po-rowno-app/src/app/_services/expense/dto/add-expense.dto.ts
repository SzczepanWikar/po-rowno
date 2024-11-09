import { Currency } from 'src/app/_common/enums/currency.enum';
import { ExpenseType } from 'src/app/_common/enums/expense-type.enum';
import { ExpenseDeptorDto } from './expense-deptor.dto';

export interface AddExpenseDto {
  /**
   * @minLength 1
   * @maxLength 50
   */
  name: string;
  /**
   * @format double
   * @min 0
   */
  amount: number;
  currency: Currency;
  type: ExpenseType;
  /** @format uuid */
  groupId: string;
  /** @minItems 1 */
  deptors: ExpenseDeptorDto[];
}
