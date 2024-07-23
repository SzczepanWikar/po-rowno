using EventStore.Client;
using Infrastructure.EventStore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Projections
{
    public class ReadModelProjector : IHostedService
    {
        private readonly EventStorePersistentSubscriptionsClient _eventStoreClient;
        private readonly EventParser _eventParser;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReadModelProjector> _logger;
        private PersistentSubscription? _subscription;
        private readonly object _resubscribeLock = new();
        private CancellationToken _cancellationToken;
        private readonly string _subscriptionGroup;

        public ReadModelProjector(
            EventStorePersistentSubscriptionsClient eventStoreClient,
            EventParser eventTypeParser,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration config,
            ILogger<ReadModelProjector> logger
        )
        {
            _eventStoreClient = eventStoreClient;
            _eventParser = eventTypeParser;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = config;
            _logger = logger;

            var group = _configuration
                .GetRequiredSection("EventStore")
                .GetValue<string>("SubscriptionGroup");

            if (group == null)
            {
                throw new Exception("Config EventStore.SubscriptionGroup must be defined");
            }

            _subscriptionGroup = group;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            _cancellationToken = cancellationToken;

            await CheckSubscritpionGroup();

            await SubscribeToEventStoreAsync();
        }

        private async Task CheckSubscritpionGroup()
        {
            var existingGroups = await _eventStoreClient.ListToAllAsync();
            var existingGroup = existingGroups
                .Where(e => e.GroupName == _subscriptionGroup)
                .FirstOrDefault();

            if (existingGroup == null)
            {
                await _eventStoreClient.CreateToAllAsync(
                    _subscriptionGroup,
                    new PersistentSubscriptionSettings(startFrom: Position.Start),
                    cancellationToken: _cancellationToken
                );
                _logger.LogInformation($"Created new Subcription Group {_subscriptionGroup}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();

            return Task.CompletedTask;
        }

        private async Task SubscribeToEventStoreAsync()
        {
            _subscription = await _eventStoreClient.SubscribeToAllAsync(
                _subscriptionGroup,
                OnEventAppered,
                cancellationToken: _cancellationToken,
                subscriptionDropped: OnSubscriptionDropped
            );

            _logger.LogInformation($"Subscribed to {_subscription}");
        }

        private async Task OnEventAppered(
            PersistentSubscription subscription,
            ResolvedEvent resolvedEvent,
            int? retryCount,
            CancellationToken cancellationToken
        )
        {
            try
            {
                _logger.LogInformation(
                    $"Event ${resolvedEvent.Event.EventId} from stream ${resolvedEvent.Event.EventStreamId} appered"
                );
                await HandleEventAsync(resolvedEvent, cancellationToken);
                var ev = resolvedEvent.Event.EventType;
                await subscription.Ack(resolvedEvent);
                _logger.LogInformation(
                    $"Event ${resolvedEvent.Event.EventId} from stream ${resolvedEvent.Event.EventStreamId} proceed correctly"
                );
            }
            catch (Exception ex)
            {
                await subscription.Nack(
                    PersistentSubscriptionNakEventAction.Park,
                    ex.Message,
                    resolvedEvent
                );
                _logger.LogInformation(
                    $"Error ocured while processing event ${resolvedEvent.Event.EventId} from stream ${resolvedEvent.Event.EventStreamId}"
                );
            }
        }

        private async Task HandleEventAsync(
            ResolvedEvent resolvedEvent,
            CancellationToken cancellationToken
        )
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                if (resolvedEvent.Event.Data.Length == 0)
                {
                    return;
                }

                var notification = _eventParser.GetEventNotification(resolvedEvent.Event);

                if (notification == null)
                {
                    return;
                }

                await mediator.Publish(notification, cancellationToken: cancellationToken);
            }
        }

        private void OnSubscriptionDropped(
            PersistentSubscription streamSubscription,
            SubscriptionDroppedReason reason,
            Exception? exception
        )
        {
            _logger.LogError("Subcription dropped!!!");
            _logger.LogError(exception?.ToString());
            var resubscribed = false;
            while (resubscribed == false)
            {
                try
                {
                    _logger.LogInformation("Trying to resubscribe");
                    Monitor.Enter(_resubscribeLock);

                    using (NoSynchonizationContextScope.Enter())
                    {
                        SubscribeToEventStoreAsync().Wait(_cancellationToken);
                    }

                    resubscribed = true;
                }
                catch (Exception err)
                {
                    _logger.LogError("Resubscription failed");
                    _logger.LogError(err.ToString());

                    resubscribed = false;
                }
                finally
                {
                    Monitor.Exit(_resubscribeLock);

                    _logger.LogInformation("Resubscribed");
                }
            }
        }
    }
}
