using System.Text.Json;
using Core.Common.Exceptions;
using Core.Common.Projections;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Projections.InternalProjections.Repository
{
    public class IndexProjectionRepository : IIndexProjectionRepository
    {
        #region Constants
        private readonly string _streamName;
        private readonly string _emailIndexedEvent;
        #endregion

        private readonly EventStoreClient _eventStoreClient;
        private readonly ILogger<IndexProjectionRepository> _logger;

        public IndexProjectionRepository(
            EventStoreClient eventStoreClient,
            ILogger<IndexProjectionRepository> logger,
            string streamName,
            string emailIndexedEvent
        )
        {
            _eventStoreClient = eventStoreClient;
            _logger = logger;
            _streamName = streamName ?? throw new ArgumentNullException(nameof(streamName));
            _emailIndexedEvent =
                emailIndexedEvent ?? throw new ArgumentNullException(nameof(emailIndexedEvent));
        }

        public async Task CheckAvailibility(
            string indexedValue,
            CancellationToken cancellationToken = default
        )
        {
            var readResult = _eventStoreClient.ReadStreamAsync(
                Direction.Backwards,
                _streamName,
                StreamPosition.End,
                cancellationToken: cancellationToken
            );

            var readState = await readResult.ReadState.ConfigureAwait(false);
            if (readState == ReadState.StreamNotFound)
            {
                return; // If Stream not found any account has been created yet
            }

            var emailLower = indexedValue.ToLower();

            await foreach (var @event in readResult)
            {
                var eventData = JsonSerializer.Deserialize<IndexedValueEvent>(
                    @event.Event.Data.Span
                );

                if (eventData.IndexedValue != emailLower)
                {
                    continue;
                }

                if (@event.Event.EventType == _emailIndexedEvent)
                {
                    throw new ConflictException("Given email is already in use.");
                }
                else
                {
                    break;
                }
            }
        }

        public async Task<Guid?> GetOwnerId(
            string indexedValue,
            CancellationToken cancellationToken = default
        )
        {
            var readResult = _eventStoreClient.ReadStreamAsync(
                Direction.Backwards,
                _streamName,
                StreamPosition.End,
                cancellationToken: cancellationToken
            );

            var readState = await readResult.ReadState.ConfigureAwait(false);

            if (readState == ReadState.StreamNotFound)
            {
                throw new Exception($"Stream {_streamName} not found");
            }

            var emailLower = indexedValue.ToLower();

            await foreach (var @event in readResult)
            {
                var eventData = JsonSerializer.Deserialize<IndexedValueEvent>(
                    @event.Event.Data.Span
                );

                if (eventData.IndexedValue.ToLower() != emailLower)
                {
                    continue;
                }

                if (@event.Event.EventType == _emailIndexedEvent)
                {
                    return eventData.OwnerId;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }
    }
}
