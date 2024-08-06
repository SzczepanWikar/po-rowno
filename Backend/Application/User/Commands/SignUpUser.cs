using Application.User.Auth;
using Core.Common.Exceptions;
using Core.User.Events;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace Application.User.Commands
{
    public record SignUpUser(
        string Username,
        string Email,
        string Password,
        string ReapetedPassword
    ) : IRequest<Guid>;

    public class SignUpUserHandler : IRequestHandler<SignUpUser, Guid>
    {
        private readonly IEventStoreRepository<Core.User.User> _repository;
        private readonly EmailConflictValidator _emailConflictValidator;

        public SignUpUserHandler(
            IEventStoreRepository<Core.User.User> repository,
            EmailConflictValidator emailConflictValidator
        )
        {
            _repository = repository;
            _emailConflictValidator = emailConflictValidator;
        }

        public async Task<Guid> Handle(SignUpUser request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ReapetedPassword)
            {
                throw new BadRequestException("Passwords are not same.");
            }

            await _emailConflictValidator.CheckAvailibility(request.Email, cancellationToken);

            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password, 13);

            var userSignedUp = new UserSignedUp(
                Guid.NewGuid(),
                request.Username,
                request.Email,
                hashedPassword
            );

            await _repository.Create(userSignedUp.Id, userSignedUp, cancellationToken);

            return userSignedUp.Id;
        }
    }
}
