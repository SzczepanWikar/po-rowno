using CommandModel.User.Services;
using Core.Common.Exceptions;
using Core.User;
using Core.User.Events;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace CommandModel.User.Commands
{
    public record ActivateAccount(Guid Token) : IRequest;

    public class ActivateAccountHandler : IRequestHandler<ActivateAccount>
    {
        private readonly IUserService _service;
        private readonly IEventStoreRepository<UserToken> _tokenRepository;

        public ActivateAccountHandler(
            IUserService service,
            IEventStoreRepository<UserToken> tokenRepository
        )
        {
            _service = service;
            _tokenRepository = tokenRepository;
        }

        public async Task Handle(ActivateAccount request, CancellationToken cancellationToken)
        {
            var token = await GetToken(request, cancellationToken);
            await ValidateUser(token.UserId, cancellationToken);

            await _service.AppendAsync(token.UserId, new AccountActivated(token.UserId));
            await _tokenRepository.AppendAsync(token.Id, new UserTokenUsed(token.Id));
        }

        private async Task<UserToken> GetToken(
            ActivateAccount request,
            CancellationToken cancellationToken
        )
        {
            var token = await _tokenRepository.FindOneAsync(request.Token, cancellationToken);

            if (token == null)
            {
                throw new NotFoundException("User Token not found.");
            }

            if (token.Type != UserTokenType.AccountActivation)
            {
                throw new NotFoundException("User Token not found.");
            }

            return token;
        }

        private async Task ValidateUser(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _service.FindOneAsync(userId, cancellationToken);

            if (user.Status != UserStatus.Inactive)
            {
                throw new BadRequestException("Incorrect Status.");
            }
        }
    }
}
