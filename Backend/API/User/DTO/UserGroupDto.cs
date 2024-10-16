using API.Group.DTO;
using ReadModel.UserGroup;

namespace API.User.DTO
{
    public sealed class UserGroupDto
    {
        public UserGroupStatus Status { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public UserDto? User { get; set; }
        public GroupDto? Group { get; set; }

        public static UserGroupDto FromEntity(UserGroupEntity entity)
        {
            UserGroupDto dto =
                new()
                {
                    Status = entity.Status,
                    UserId = entity.UserId,
                    GroupId = entity.GroupId,
                };

            if (entity.User is not null)
            {
                dto.User = UserDto.FromEntity(entity.User);
            }

            if (entity.Group is not null)
            {
                dto.Group = GroupDto.FromEntity(entity.Group);
            }

            return dto;
        }
    }
}
