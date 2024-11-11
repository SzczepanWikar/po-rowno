export interface ChangePasswordDto {
  /** @minLength 1 */
  oldPassword: string;
  /** @minLength 1 */
  newPassword: string;
}
