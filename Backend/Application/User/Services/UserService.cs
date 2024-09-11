using Application.User.Commands;
using Core.Common.Exceptions;
using Core.User.Events;
using Infrastructure.EventStore.Repository;
using Microsoft.AspNetCore.Identity;

namespace Application.User.Services
{
    using User = Core.User.User;

    public class UserService : IUserService
    {
        private readonly IEventStoreRepository<User> _repository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(
            IEventStoreRepository<User> repository,
            IPasswordHasher<User> passwordHasher
        )
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> CreateAsync(
            SignUpUser command,
            CancellationToken cancellationToken = default
        )
        {
            var id = Guid.NewGuid();

            var user = new User();

            var hashedPassword = _passwordHasher.HashPassword(user, command.Password);

            var userSignedUp = new UserSignedUp(
                id,
                command.Username,
                command.Email,
                hashedPassword
            );

            await _repository.CreateAsync(id, userSignedUp, cancellationToken);

            return id;
        }

        public async Task<User> FindOneAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _repository.FindOneAsync(id, cancellationToken);

            if (user == null || user.Deleted)
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
            await _repository.AppendAsync(id, @event, cancellationToken);
        }

        public async Task AppendAsync(
            Guid id,
            IReadOnlyCollection<object> events,
            CancellationToken cancellationToken = default
        )
        {
            await _repository.AppendAsync(id, @events, cancellationToken);
        }
    }
}
