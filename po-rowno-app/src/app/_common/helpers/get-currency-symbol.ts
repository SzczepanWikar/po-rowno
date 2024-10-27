import { Currency } from '../enums/currency.enum';

const currenciesSymbols: Record<Currency, string> = {
  [Currency.Dollar]: '$',
  [Currency.Euro]: '€',
  [Currency.PolishZloty]: 'zł',
} as const;

export function getCurrencySymbol(currency: Currency): string {
  return currenciesSymbols[currency];
}
