using Core.Common.Aggregate;

namespace Infrastructure.EventStore.Repository
{
    public interface IEventStoreRepository<T>
        where T : class, IAggregate
    {
        Task CreateAsync(Guid id, object @event, CancellationToken ct = default);
        Task CreateAsync(
            Guid id,
            IReadOnlyCollection<object> @events,
            CancellationToken ct = default
        );
        Task AppendAsync(Guid id, object @event, CancellationToken ct = default);
        Task AppendAsync(
            Guid id,
            IReadOnlyCollection<object> @events,
            CancellationToken ct = default
        );
        Task<T?> FindOneAsync(
            Guid id,
            CancellationToken cancellationToken = default,
            ulong? fromVersion = null
        );
    }
}
