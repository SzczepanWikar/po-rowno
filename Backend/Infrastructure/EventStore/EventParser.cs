using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Core.Common;
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
                return null;
            }

            var eventData = GetEventData(@event, eventType);

            if (eventData == null)
            {
                return null;
            }

            return GetEventNotification(eventData, eventType);
        }

        public object? GetEventNotification(object eventData, Type eventType)
        {
            var notificationType = typeof(EventNotification<>).MakeGenericType(eventType);
            var notification = Activator.CreateInstance(notificationType, new object[] { eventData });

            return notification;
        }
    }
}