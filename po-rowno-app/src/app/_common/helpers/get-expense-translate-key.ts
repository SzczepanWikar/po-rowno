import { ExpenseType } from '../enums/expense-type.enum';

const expenseTypeRecord: Record<ExpenseType, string> = {
  [ExpenseType.Cost]: 'COST',
  [ExpenseType.Settlement]: 'SETTLEMENT',
} as const;

export function getExpenseTypeTranslateKey(et: ExpenseType): string {
  return 'EXPENSE_TYPE.' + expenseTypeRecord[et];
}
