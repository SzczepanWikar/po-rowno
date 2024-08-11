using System.Text.Json;
using Core.Common.Exceptions;
using Core.Common.Projections;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace Application.User.Auth
{
    public class EmailConflictValidator
    {
        #region Constants
        private const string StreamName = $"{InternalProjectionName.EmailIndex}-res";
        private const string EmailIndexedEvent = "UserEmailIndexed";
        #endregion

        private readonly EventStoreClient _eventStoreClient;
        private readonly ILogger<EmailConflictValidator> _logger;

        public EmailConflictValidator(
            EventStoreClient eventStoreClient,
            ILogger<EmailConflictValidator> logger
        )
        {
            _eventStoreClient = eventStoreClient;
            _logger = logger;
        }

        public async Task CheckAvailibility(
            string email,
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
                return; // If Stream not found any account has been created yet
            }

            var emailLower = email.ToLower();

            await foreach (var @event in readResult)
            {
                var eventData = JsonSerializer.Deserialize<IndexedEmail>(@event.Event.Data.Span);

                if (eventData.Email != emailLower)
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

        public async Task<Guid?> GetUserId(
            string email,
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

            var emailLower = email.ToLower();

            await foreach (var @event in readResult)
            {
                var eventData = JsonSerializer.Deserialize<IndexedEmail>(@event.Event.Data.Span);

                if (eventData.Email.ToLower() != emailLower)
                {
                    continue;
                }

                if (@event.Event.EventType == EmailIndexedEvent)
                {
                    return eventData.UserId;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        private struct IndexedEmail
        {
            public string Email { get; set; }
            public Guid? UserId { get; set; }
        }
    }
}
