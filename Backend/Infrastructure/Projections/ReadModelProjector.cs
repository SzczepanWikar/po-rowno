using Core.Common;
using EventStore.Client;
using Infrastructure.EventStore;
using MediatR;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace Infrastructure.Projections
{
    public class ReadModelProjector : IHostedService
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly IMediator _mediator;
        private readonly EventParser _eventParser;
        private StreamSubscription? _subscription;
        private readonly object _resubscribeLock = new();
        private CancellationToken _cancellationToken;

        public ReadModelProjector(EventStoreClient eventStoreClient, EventParser eventTypeParser, IMediator mediator)
        {
            _eventStoreClient = eventStoreClient;
            _eventParser = eventTypeParser;
            _mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            _cancellationToken = cancellationToken;
            await SubscribeToEventStoreAsync();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();

            return Task.CompletedTask;
        }

        private async Task SubscribeToEventStoreAsync()
        {
            _subscription = await _eventStoreClient.SubscribeToAllAsync(
                FromAll.Start,
                OnEventAppered,
                cancellationToken: _cancellationToken,
                subscriptionDropped: OnSubscriptionDropped
            );
        }

        private async Task OnEventAppered(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
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

            await _mediator.Publish(notification, cancellationToken: cancellationToken);
        }

        private void OnSubscriptionDropped(StreamSubscription streamSubscription, SubscriptionDroppedReason reason, Exception? exception)
        {
            var resubscribed = false;
            while (resubscribed == false)
            {
                try
                {
                    Monitor.Enter(_resubscribeLock);

                    using (NoSynchonizationContextScope.Enter())
                    {
                        SubscribeToEventStoreAsync().Wait(_cancellationToken);
                    }

                    resubscribed = true;
                }
                catch (Exception)
                {
                    resubscribed = false;
                }
                finally
                {
                    Monitor.Exit(_resubscribeLock);
                }
            }
        }
    }
}