
namespace Core.Common.Aggregate
{
    public abstract class Aggregate : IAggregate
    {
        public Guid Id { get; protected set; }
        public bool Deleted { get; protected set; } = false;

        public abstract void When(object @event);
    }
}
