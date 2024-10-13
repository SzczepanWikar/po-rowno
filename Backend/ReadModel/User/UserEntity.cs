using Core.User;
using ReadModel.Expense;
using ReadModel.Group;

namespace ReadModel.User
{
    public sealed class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public ICollection<UserGroupEntity> UserGroups { get; set; }
        public ICollection<ExpenseEntity> Expenses { get; set; }
        public ICollection<ExpenseDeptorEntity> Depts { get; set; }
        public ICollection<GroupEntity> OwnedGroups { get; set; }
        public ICollection<BalanceEntity> CreditBalances { get; set; }
        public ICollection<BalanceEntity> DeptBalances { get; set; }
    }
}
