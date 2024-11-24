using QueryModel.Group;
using QueryModel.User;

namespace QueryModel.UserGroup
{
    public enum UserGroupStatus
    {
        Active,
        Banned,
        Leaved,
    }

    public class UserGroupEntity
    {
        public UserGroupStatus Status { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public UserEntity User { get; set; }
        public GroupEntity Group { get; set; }
    }
}
