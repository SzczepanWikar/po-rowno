using Core.Common.Exceptions;
using Core.User;
using Core.User.Events;
using Core.User.UserToken;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace Application.User.Commands
{
    using User = Core.User.User;

    public record ActivateAccount(Guid Token) : IRequest;

    public class ActivateAccountHandler : IRequestHandler<ActivateAccount>
    {
        private readonly IEventStoreRepository<User> _repository;
        private readonly IEventStoreRepository<UserToken> _tokenRepository;

        public ActivateAccountHandler(
            IEventStoreRepository<User> repository,
            IEventStoreRepository<UserToken> tokenRepository
        )
        {
            _repository = repository;
            _tokenRepository = tokenRepository;
        }

        public async Task Handle(ActivateAccount request, CancellationToken cancellationToken)
        {
            var token = await GetToken(request, cancellationToken);
            await ValidateUser(token.UserId, cancellationToken);

            await _repository.Append(token.UserId, new AccountActivated(token.UserId));
            await _tokenRepository.Append(token.Id, new UserTokenUsed(token.Id));
        }

        private async Task<UserToken> GetToken(
            ActivateAccount request,
            CancellationToken cancellationToken
        )
        {
            var token = await _tokenRepository.Find(request.Token, cancellationToken);

            if (token == null)
            {
                throw new NotFoundException("User Token not found.");
            }

            return token;
        }

        private async Task ValidateUser(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _repository.Find(userId, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (user.Status != UserStatus.Inactive)
            {
                throw new BadRequestException("Incorrect Status.");
            }
        }
    }
}
