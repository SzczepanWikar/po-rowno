export interface UpdateGroupDto {
  /**
   * @minLength 0
   * @maxLength 50
   */
  name?: string | null;
  /**
   * @minLength 0
   * @maxLength 400
   */
  description?: string | null;
  /** @format uuid */
  ownerId?: string | null;
}
