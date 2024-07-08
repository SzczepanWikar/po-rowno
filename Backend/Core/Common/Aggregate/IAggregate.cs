namespace Core.Common.Aggregate
{
    public interface IAggregate
    {
        Guid Id { get; }
        bool Deleted { get; }

        void When(object @event);
    }
}
