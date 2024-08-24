using Application.User.Commands;
using Core.Common.Exceptions;
using Core.User.Events;
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

        public async Task<Guid> CreateAsync(
            SignUpUser command,
            CancellationToken cancellationToken = default
        )
        {
            var id = new Guid();

            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(command.Password, 10);

            var userSignedUp = new UserSignedUp(
                id,
                command.Username,
                command.Email,
                hashedPassword
            );

            await _repository.Create(id, userSignedUp, cancellationToken);

            return id;
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

        public async Task AppendAsync(
            Guid id,
            object @event,
            CancellationToken cancellationToken = default
        )
        {
            await _repository.Append(id, @event, cancellationToken);
        }
    }
}
