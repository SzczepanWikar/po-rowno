using System.Text.Json;
using Core.Common.Exceptions;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Projections.InternalProjections.Repository
{
    public abstract class IndexProjectionRepository : IIndexProjectionRepository
    {
        #region Constants
        protected abstract string StreamName { get; init; }
        protected abstract string EmailIndexedEvent { get; init; }
        #endregion

        private readonly EventStoreClient _eventStoreClient;
        private readonly ILogger<IndexProjectionRepository> _logger;

        public IndexProjectionRepository(
            EventStoreClient eventStoreClient,
            ILogger<IndexProjectionRepository> logger
        )
        {
            _eventStoreClient = eventStoreClient;
            _logger = logger;
        }

        public async Task CheckAvailibility(
            string indexedValue,
            CancellationToken cancellationToken = default
        )
        {
            var readResult = _eventStoreClient.ReadStreamAsync(
                Direction.Backwards,
                StreamName,
                StreamPosition.End,
                cancellationToken: cancellationToken
            );

            var readState = await readResult.ReadState.ConfigureAwait(false);
            if (readState == ReadState.StreamNotFound)
            {
                return; // If Stream not found any stream has been created yet
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

                if (@event.Event.EventType == EmailIndexedEvent)
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
                StreamName,
                StreamPosition.End,
                cancellationToken: cancellationToken
            );

            var readState = await readResult.ReadState.ConfigureAwait(false);

            if (readState == ReadState.StreamNotFound)
            {
                throw new Exception($"Stream {StreamName} not found");
            }

            var emailLower = indexedValue.ToLower();

            await foreach (var @event in readResult)
            {
                var eventData = JsonSerializer.Deserialize<IndexedValueEvent>(
                    @event.Event.Data.Span
                );

                if (eventData.IndexedValue?.ToLower() != emailLower)
                {
                    continue;
                }

                if (@event.Event.EventType == EmailIndexedEvent)
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
