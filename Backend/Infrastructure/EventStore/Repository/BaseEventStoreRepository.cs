using Core.Common.Aggregate;
using Core.Foo;
using EventStore.Client;
using System.Text.Json;

namespace Infrastructure.EventStore.Repository
{
    public class BaseEventStoreRepository<T> : IEventStoreRepository<T> where T : class, IAggregate
    {
        private readonly EventStoreClient _eventStoreClient;

        public BaseEventStoreRepository(EventStoreClient eventStoreClient)
        {
            _eventStoreClient = eventStoreClient;
        }

        public Task<ulong> Append(Guid id, object @event, CancellationToken ct = default)
        {
            return Append(id, [@event], ct);
        }

        public async Task<ulong> Append(Guid id, IEnumerable<object> @events, CancellationToken ct = default)
        {
            var eventsToAppend = @events.Select(e => ObjectToEventData(e));

            var res = await _eventStoreClient.AppendToStreamAsync(GetStreamId(id), StreamState.NoStream, eventsToAppend, cancellationToken: ct);

            return res.NextExpectedStreamRevision.ToUInt64();
        }

        public Task<ulong> Append(Guid id, object @event, ulong version, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> Append(Guid id, IEnumerable<object> events, ulong version, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> Find(Guid id, CancellationToken cancellationToken, ulong? fromVersion)
        {
            var readResult = _eventStoreClient.ReadStreamAsync(Direction.Forwards, GetStreamId(id), fromVersion ?? StreamPosition.Start, cancellationToken: cancellationToken);

            var readState = await readResult.ReadState.ConfigureAwait(false);
            if (readState == ReadState.StreamNotFound)
            {
                return null;
            }

            var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;

            await foreach (var @event in readResult)
            {
                var evt = @event.Event;
                var eventType = GetEventType(evt.EventType);

                if (eventType == null)
                {
                    continue;
                }

                var eventData = JsonSerializer.Deserialize(evt.Data.Span, eventType);

                if (eventData == null)
                {
                    continue;
                }

                aggregate.When(eventData);

            }

            return aggregate;
        }

        private static Type? GetEventType(string eventType)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(x => x.FullName == eventType || x.Name == eventType)).FirstOrDefault();
        }

        private EventData ObjectToEventData(object @event)
        {
            return new EventData(Uuid.NewUuid(), @event.GetType().Name, JsonSerializer.SerializeToUtf8Bytes(@event));

        }

        private string GetStreamId(Guid id)
        {
            return typeof(T).FullName + "_" + id;
        }
    }
}
