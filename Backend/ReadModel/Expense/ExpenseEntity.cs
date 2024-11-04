using Core.Expense;
using Core.Group;
using ReadModel.Group;
using ReadModel.User;

namespace ReadModel.Expense
{
    public sealed class ExpenseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public ExpenseType Type { get; set; }
        public Guid PayerId { get; set; }
        public Guid GroupId { get; set; }
        public string? PaymentId { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public GroupEntity Group { get; set; }
        public UserEntity Payer { get; set; }
        public ICollection<ExpenseDeptorEntity> Deptors { get; set; }
    }
}
