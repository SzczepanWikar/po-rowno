using System.Text.Json;
using Core.Common.Exceptions;
using Core.Common.Projections;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace Application.User.Auth
{
    public class EmailConflictValidator
    {
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
            try
            {
                var readResult = _eventStoreClient.ReadStreamAsync(
                    Direction.Backwards,
                    $"{InternalProjectionName.EmailIndex}-res",
                    StreamPosition.End,
                    cancellationToken: cancellationToken
                );
                var readState = await readResult.ReadState.ConfigureAwait(false);
                if (readState == ReadState.StreamNotFound)
                {
                    throw new Exception($"{InternalProjectionName.EmailIndex}-res not found.");
                }

                var emailLower = email.ToLower();

                await foreach (var @event in readResult)
                {
                    var eventData = JsonSerializer.Deserialize<IndexedEmail>(
                        @event.Event.Data.Span
                    );

                    if (
                        eventData.Email == emailLower
                        && @event.Event.EventType == "UserEmailIndexed"
                    )
                    {
                        throw new ConflictException("Given email is already in use.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private struct IndexedEmail
        {
            public string Email { get; set; }
        }
    }
}
