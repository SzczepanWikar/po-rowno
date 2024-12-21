using System.Text.Json;
using Core.Common.Aggregate;
using EventStore.Client;

namespace Infrastructure.EventStore.Repository
{
    public class BaseEventStoreRepository<T> : IEventStoreRepository<T>
        where T : class, IAggregate
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly EventParser _eventTypeParser;

        public BaseEventStoreRepository(
            EventStoreClient eventStoreClient,
            EventParser eventTypeParser
        )
        {
            _eventStoreClient = eventStoreClient;
            _eventTypeParser = eventTypeParser;
        }

        public Task CreateAsync(Guid id, object @event, CancellationToken ct = default)
        {
            return CreateAsync(id, new[] { @event }, ct);
        }

        public async Task CreateAsync(
            Guid id,
            IReadOnlyCollection<object> events,
            CancellationToken ct = default
        )
        {
            await AppendToStreamAsync(id, events, StreamState.NoStream, ct);
        }

        public Task AppendAsync(Guid id, object @event, CancellationToken ct = default)
        {
            return AppendAsync(id, [@event], ct);
        }

        public async Task AppendAsync(
            Guid id,
            IReadOnlyCollection<object> @events,
            CancellationToken ct = default
        )
        {
            await AppendToStreamAsync(id, @events, StreamState.StreamExists, ct);
        }

        public async Task<T?> FindOneAsync(
            Guid id,
            CancellationToken cancellationToken = default,
            ulong? fromVersion = null
        )
        {
            var readResult = _eventStoreClient.ReadStreamAsync(
                Direction.Forwards,
                GetStreamId(id),
                fromVersion ?? StreamPosition.Start,
                cancellationToken: cancellationToken
            );

            var readState = await readResult.ReadState.ConfigureAwait(false);
            if (readState == ReadState.StreamNotFound)
            {
                return null;
            }

            var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;

            await foreach (var @event in readResult)
            {
                try
                {
                    var evt = @event.Event;

                    var eventData = _eventTypeParser.GetEventData(evt, null);

                    if (eventData == null)
                    {
                        continue;
                    }

                    aggregate.When(eventData);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return aggregate;
        }

        private async Task AppendToStreamAsync(
            Guid id,
            IReadOnlyCollection<object> @events,
            StreamState streamState,
            CancellationToken ct = default
        )
        {
            var eventsToAppend = @events.Select(e => ObjectToEventData(e));

            var res = await _eventStoreClient.AppendToStreamAsync(
                GetStreamId(id),
                streamState,
                eventsToAppend,
                cancellationToken: ct
            );
        }

        private EventData ObjectToEventData(object @event)
        {
            return new EventData(
                Uuid.NewUuid(),
                @event.GetType().Name,
                JsonSerializer.SerializeToUtf8Bytes(@event)
            );
        }

        private string GetStreamId(Guid id)
        {
            return typeof(T).FullName + "_" + id;
        }
    }
}
