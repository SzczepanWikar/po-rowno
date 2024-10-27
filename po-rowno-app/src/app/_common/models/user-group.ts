import { UserGroupStatus } from '../enums/user-group-status.enum';
import { Group } from './group';
import { User } from './user';

export interface UserGroup {
  status: UserGroupStatus;
  userId: string;
  groupId: string;
  user?: User;
  group?: Group;
}
