using Core.User;
using DatabaseProjections.Expense;
using DatabaseProjections.Group;

namespace DatabaseProjections.User
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
    }
}
