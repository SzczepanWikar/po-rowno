using Core.Group;
using DatabaseProjections.User;

namespace DatabaseProjections.Group
{
    public sealed class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Currency Currency { get; set; }
        public UserEntity Owner { get; set; }
        public ICollection<UserGroupEntity> UserGroups { get; set; }
    }
}
