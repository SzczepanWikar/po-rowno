using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Core.Common.Projections;
using EventStore.Client;

namespace Infrastructure.EventStore
{
    public sealed class EventParser
    {
        public static readonly EventParser Instance = new();
        private readonly ConcurrentDictionary<string, Type?> _eventTypesDictionary = new();
        private readonly ConcurrentDictionary<string, Type?> _eventNotificationsDictionary = new();

        public Type? GetEventType(string name)
        {
            if (_eventTypesDictionary.ContainsKey(name))
            {
                return _eventTypesDictionary[name];
            }

            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.GetTypes().Where(f => f.FullName == name || f.Name == name)).FirstOrDefault();

            _eventTypesDictionary.TryAdd(name, type);

            return type;
        }

        public object? GetEventData(EventRecord @event, Type? eventType)
        {
            if (eventType == null)
            {
                eventType = this.GetEventType(@event.EventType);

                if (eventType == null)
                {
                    return null;
                }
            }

            var eventData = JsonSerializer.Deserialize(@event.Data.Span, eventType);

            return eventData;
        }

        public object? GetEventNotification(EventRecord @event)
        {
            Type? eventType = GetEventType(@event.EventType);

            if (eventType == null)
            {
                if(!_eventNotificationsDictionary.ContainsKey(@event.EventType))
                {
                    _eventNotificationsDictionary.TryAdd(@event.EventType, null);
                }

                return null;
            }

            var notificationType = GetNotificationType(@event.EventType, eventType);

            if(notificationType == null)
            {
                return null;
            }

            var eventData = GetEventData(@event, eventType);

            if (eventData == null)
            {
                return null;
            }

            var notification = Activator.CreateInstance(notificationType, new object[] { eventData });

            return notification;
        }

        private Type? GetNotificationType(string name, Type eventType)
        {
            Type? notificationType;

            if (_eventNotificationsDictionary.ContainsKey(name))
            {
                 notificationType = _eventNotificationsDictionary[name];
            }
            else
            {
                notificationType = typeof(EventNotification<>).MakeGenericType(eventType);
                _eventNotificationsDictionary.TryAdd(name, notificationType);
                
            }
            
            return notificationType;
        }
    }
}