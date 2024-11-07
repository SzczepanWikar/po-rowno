import { Currency } from 'src/app/_common/enums/currency.enum';

export interface CreateGroupDto {
  /**
   * @minLength 0
   * @maxLength 50
   */
  name: string;
  /**
   * @minLength 0
   * @maxLength 400
   */
  description: string;
  currency: Currency;
}
