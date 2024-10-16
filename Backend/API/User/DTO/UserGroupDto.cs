using API.Group.DTO;
using ReadModel.Expense;
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
            var dto = InitSimpleFields(entity);

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

        public static UserGroupDto FromGroupRelatedEntity(UserGroupEntity entity)
        {
            var dto = InitSimpleFields(entity);

            if (entity.User is not null)
            {
                dto.User = UserDto.FromUserGroupRelatedEntity(entity.User);
            }

            return dto;
        }

        public static UserGroupDto FromUserRelatedEntity(UserGroupEntity entity)
        {
            var dto = InitSimpleFields(entity);

            if (entity.Group is not null)
            {
                dto.Group = GroupDto.FromUserGroupRelatedEntity(entity.Group);
            }

            return dto;
        }

        public static UserGroupDto FromEntityWithoutRelations(UserGroupEntity entity)
        {
            return InitSimpleFields(entity);
        }

        private static UserGroupDto InitSimpleFields(UserGroupEntity entity)
        {
            return new()
            {
                Status = entity.Status,
                UserId = entity.UserId,
                GroupId = entity.GroupId,
            };
        }
    }
}
