import { UserStatus } from '../enums/user-status.enum';
import { UserGroup } from './user-group';

export interface User {
  id: string;
  username: string;
  email: string;
  status: UserStatus;
  userGroups?: UserGroup[];
}
