import { Currency } from '../enums/currency.enum';
import { Balance } from './balance';
import { User } from './user';
import { UserGroup } from './user-group';

export interface Group {
  id: string;
  name: string;
  description: string;
  joinCode: string;
  joinCodeValidTo: Date;
  currency: Currency;
  ownerId: string;
  owner?: User;
  userGroups?: UserGroup[];
  balances?: Balance[];
}
