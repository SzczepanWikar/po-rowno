using Core.Common.Aggregate;
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
        public IList<Guid> OwnersIds { get; private set; } = new List<Guid>();

        public override void When(object @event)
        {
            throw new NotImplementedException();
        }
    }
}
