using Core.Common.Aggregate;
using Core.Expense.Events;
using Core.Group;

namespace Core.Expense
{
    public enum ExpenseType
    {
        Cost = 0,
        Settlement
    }

    public sealed class Expense : Aggregate
    {
        public string Name { get; private set; } = String.Empty;
        public decimal Amount { get; private set; }
        public Currency Currency { get; private set; }
        public ExpenseType Type { get; private set; }
        public Guid GroupId { get; private set; }
        public Guid PayerId { get; private set; }
        public IList<Guid> DeptorsIds { get; private set; } = new List<Guid>();

        public override void When(object @event)
        {
            switch (@event)
            {
                case ExpenseCreated e:
                    Id = e.Id;
                    Name = e.Name;
                    Amount = e.Amount;
                    Currency = e.Currency;
                    Type = e.Type;
                    GroupId = e.GroupId;
                    PayerId = e.PayerId;
                    DeptorsIds = e.DeptorsIds;
                    break;
                case ExpenseRemoved:
                    Deleted = true;
                    break;
            }
        }
    }
}
