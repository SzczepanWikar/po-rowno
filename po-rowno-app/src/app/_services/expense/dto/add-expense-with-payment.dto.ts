import { Currency } from 'src/app/_common/enums/currency.enum';

export interface AddExpenseWithPaymentDto {
  /**
   * @format double
   * @min 0
   */
  amount: number;
  currency: Currency;
  /** @format uuid */
  groupId: string;
  /** @format uuid */
  receiverId: string;
}
