using Core.Common.Aggregate;
using EventStore.Client;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Infrastructure.EventStore.Repository
{
    public class BaseEventStoreRepository<T> : IEventStoreRepository<T> where T : class, IAggregate
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly EventTypeParser _eventTypeParser;
        public BaseEventStoreRepository(EventStoreClient eventStoreClient, EventTypeParser eventTypeParser)
        {
            _eventStoreClient = eventStoreClient;
            _eventTypeParser = eventTypeParser;
        }

        public Task Create(Guid id, object @event, CancellationToken ct = default)
        {
            return Create(id, new[]{ @event }, ct);
        }

        public async Task Create(Guid id, IEnumerable<object> events, CancellationToken ct = default)
        {
            await AppendToStream(id, events, StreamState.NoStream, ct);
        }

        public Task Append(Guid id, object @event, CancellationToken ct = default)
        {
            return Append(id, [@event], ct);
        }

        public async Task Append(Guid id, IEnumerable<object> @events, CancellationToken ct = default)
        {
            await AppendToStream(id, @events, StreamState.StreamExists, ct);

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
                var eventType = _eventTypeParser.GetEventType(evt.EventType);

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

        private async Task AppendToStream(Guid id, IEnumerable<object> @events, StreamState streamState, CancellationToken ct = default)
        {
            var eventsToAppend = @events.Select(e => ObjectToEventData(e));

            var res = await _eventStoreClient.AppendToStreamAsync(GetStreamId(id), streamState, eventsToAppend, cancellationToken: ct);
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
