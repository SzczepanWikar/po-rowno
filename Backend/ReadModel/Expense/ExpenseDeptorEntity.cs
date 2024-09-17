using ReadModel.User;

namespace ReadModel.Expense
{
    public sealed class ExpenseDeptorEntity
    {
        public Guid Id { get; set; }
        public Guid ExpenseId { get; set; }
        public Guid? UserId { get; set; }
        public ExpenseEntity Expense { get; set; }
        public UserEntity User { get; set; }
    }
}
