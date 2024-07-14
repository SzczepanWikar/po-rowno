using Core.Common;
using EventStore.Client;
using Infrastructure.EventStore;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Projections
{
    public class ReadModelProjector : IHostedService
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly IMediator _mediator;
        private readonly EventParser _eventParser;
        private StreamSubscription? _subscription;

        public ReadModelProjector(EventStoreClient eventStoreClient, EventParser eventTypeParser, IMediator mediator)
        {
            _eventStoreClient = eventStoreClient;
            _eventParser = eventTypeParser;
            _mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            _subscription = await _eventStoreClient.SubscribeToAllAsync(
                FromAll.Start,
                HandleEvent,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();

            return Task.CompletedTask;
        }

        private async Task HandleEvent(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
        {
            if (resolvedEvent.Event.Data.Length == 0)
            {
                return;
            }

            var notification = _eventParser.GetEventNotification(resolvedEvent.Event);

            if (notification == null)
            {
                return;
            }

            await this._mediator.Publish(notification, cancellationToken: cancellationToken);
        }
    }
}