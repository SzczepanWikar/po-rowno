using Core.Common;
using Core.User;
using ReadModel.Group;
using ReadModel.User;
using ReadModel.UserGroup;

namespace API.User.DTO
{
    public sealed class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public ICollection<UserGroupDto> UserGroups { get; set; } = new List<UserGroupDto>();

        public static UserDto FromEntity(UserEntity entity)
        {
            UserDto user =
                new()
                {
                    Id = entity.Id,
                    Username = entity.Username,
                    Email = entity.Email,
                    Status = entity.Status,
                };

            if (entity.UserGroups.Count > 0)
            {
                user.UserGroups = entity
                    .UserGroups.Select(e => UserGroupDto.FromEntity(e))
                    .ToArray();
            }

            return user;
        }
    }
}
