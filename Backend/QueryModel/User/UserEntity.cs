using Core.User;
using QueryModel.Expense;
using QueryModel.Group;
using QueryModel.UserGroup;

namespace QueryModel.User
{
    public sealed class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public bool Deleted { get; set; } = false;
        public ICollection<UserGroupEntity> UserGroups { get; set; }
        public ICollection<ExpenseEntity> Expenses { get; set; }
        public ICollection<ExpenseDeptorEntity> Depts { get; set; }
        public ICollection<GroupEntity> OwnedGroups { get; set; }
        public ICollection<BalanceEntity> CreditBalances { get; set; }
        public ICollection<BalanceEntity> DeptBalances { get; set; }

        public void Delete()
        {
            Username = "USER_DELETED";
            Email = string.Empty;
            Status = UserStatus.Blocked;
            Deleted = true;

            if (UserGroups is null)
            {
                return;
            }

            if (UserGroups.Count == 0)
            {
                return;
            }

            foreach (var userGroup in UserGroups)
            {
                userGroup.Status = UserGroupStatus.Leaved;
            }
        }
    }
}
