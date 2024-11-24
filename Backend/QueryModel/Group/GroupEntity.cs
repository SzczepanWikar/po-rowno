using Core.Group;
using QueryModel.Expense;
using QueryModel.User;
using QueryModel.UserGroup;

namespace QueryModel.Group
{
    public sealed class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? JoinCode { get; set; }
        public DateTime? JoinCodeValidTo { get; set; }
        public Currency Currency { get; set; }
        public Guid OwnerId { get; set; }
        public UserEntity Owner { get; set; }
        public ICollection<UserGroupEntity> UserGroups { get; set; }
        public ICollection<ExpenseEntity> Expenses { get; set; }
        public ICollection<BalanceEntity> Balances { get; set; }
    }
}
