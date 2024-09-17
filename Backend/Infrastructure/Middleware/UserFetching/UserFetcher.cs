using Core.Common.Exceptions;
using Core.User;
using Infrastructure.EventStore.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Middleware.UserFetching
{
    public sealed class UserFetcher
    {
        private readonly IEventStoreRepository<User> _eventStoreRepository;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public UserFetcher(IEventStoreRepository<User> eventStoreRepository, IMemoryCache cache)
        {
            _eventStoreRepository = eventStoreRepository;
            _cache = cache;
        }

        public async Task<User> FetchAsync(Guid id)
        {
            if (!_cache.TryGetValue(id, out User user))
            {
                user = await _eventStoreRepository.FindOneAsync(id);

                if (user == null)
                {
                    throw new NotFoundException("User not found!");
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration,
                    SlidingExpiration = TimeSpan.FromMinutes(5),
                };

                _cache.Set(id, user, cacheEntryOptions);
            }

            return user;
        }
    }
}
