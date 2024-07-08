using Core.Common.Aggregate;

namespace Infrastructure.EventStore.Repository
{
    public interface IEventStoreRepository<T> where T : class, IAggregate
    {
        Task<ulong> Append(Guid id, object @event, CancellationToken ct = default);
        Task<ulong> Append(Guid id, IEnumerable<object> @events, CancellationToken ct = default);
        Task<ulong> Append(Guid id, object @event, ulong version, CancellationToken ct = default);
        Task<ulong> Append(Guid id, IEnumerable<object> @events, ulong version, CancellationToken ct = default);
        Task<T?> Find(Guid id, CancellationToken cancellationToken, ulong? fromVersion = null);

    }
}
