using Core.Group;
using ReadModel.User;

namespace ReadModel.Group
{
    public sealed class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? JoinCode { get; set; }
        public DateTime? JoinCodeValidTo { get; set; }
        public Currency Currency { get; set; }
        public UserEntity Owner { get; set; }
        public ICollection<UserGroupEntity> UserGroups { get; set; }
    }
}
