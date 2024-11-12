using ReadModel.Group;
using ReadModel.User;

namespace ReadModel.Expense
{
    public sealed class BalanceEntity
    {
        public Guid Id { get; set; }
        public Guid PayerId { get; set; }
        public Guid DeptorId { get; set; }
        public Guid GroupId { get; set; }
        public decimal Balance { get; set; }
        public UserEntity Payer { get; set; }
        public UserEntity Deptor { get; set; }
        public GroupEntity Group { get; set; }
    }
}
