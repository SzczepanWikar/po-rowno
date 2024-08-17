using Core.Common.Exceptions;
using Infrastructure.EventStore.Repository;

namespace Application.User
{
    using User = Core.User.User;

    public class UserService : IUserService
    {
        private readonly IEventStoreRepository<User> _repository;

        public UserService(IEventStoreRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<User> FindOneAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _repository.Find(id, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("User not found!");
            }

            return user;
        }
    }
}
