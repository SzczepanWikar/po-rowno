import { UserStatus } from 'src/app/_common/enums/user-status.enum';

export interface AppSignInResult {
  id: string;
  status: UserStatus;
  accessToken: string;
  refreshToken: string;
}
