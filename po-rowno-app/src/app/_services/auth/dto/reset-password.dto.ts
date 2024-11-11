export interface ResetPasswordDto {
  /**
   * @format email
   * @minLength 1
   */
  email: string;
  /** @minLength 1 */
  password: string;
  /** @minLength 1 */
  confirmPassword: string;
  /** @minLength 1 */
  code: string;
}
