using System.Collections.Concurrent;

namespace Infrastructure.EventStore
{
    public sealed class EventTypeParser
    {
        public static readonly EventTypeParser Instance = new();
        private readonly ConcurrentDictionary<string, Type?> _eventTypeDictionary = new();

        public Type? GetEventType(string name)
        {
            if (_eventTypeDictionary.ContainsKey(name))
            {
                return _eventTypeDictionary[name];
            }

            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(x => x.FullName == name || x.Name == name)).FirstOrDefault();


            _eventTypeDictionary.TryAdd(name, type);

            return type;
        }

    }
}
