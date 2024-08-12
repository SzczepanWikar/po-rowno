using Core.Common.Aggregate;

namespace Infrastructure.EventStore.Repository
{
    public interface IEventStoreRepository<T>
        where T : class, IAggregate
    {
        Task Create(Guid id, object @event, CancellationToken ct = default);
        Task Create(Guid id, IEnumerable<object> @events, CancellationToken ct = default);
        Task Append(Guid id, object @event, CancellationToken ct = default);
        Task Append(Guid id, IEnumerable<object> @events, CancellationToken ct = default);
        Task<T?> Find(
            Guid id,
            CancellationToken cancellationToken = default,
            ulong? fromVersion = null
        );
    }
}
