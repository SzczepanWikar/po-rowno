using Core.Common.Exceptions;
using Core.User.Events;
using Core.UserGroupEvents;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WriteModel.Group;
using WriteModel.User.Services;

namespace WriteModel.User.Commands
{
    using User = Core.User.User;

    public sealed record DeleteAccount(string Password, User User) : IRequest;

    public sealed class DeleteAccountHandler : IRequestHandler<DeleteAccount>
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAuthTokenService _authTokenService;
        private readonly IGroupService _groupService;

        public DeleteAccountHandler(
            IUserService userService,
            IPasswordHasher<User> passwordHasher,
            IAuthTokenService authTokenService,
            IGroupService groupService
        )
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _authTokenService = authTokenService;
            _groupService = groupService;
        }

        public async Task Handle(DeleteAccount request, CancellationToken cancellationToken)
        {
            ValidatePassword(request);
            await AppendEvents(request, cancellationToken);
        }

        private void ValidatePassword(DeleteAccount request)
        {
            var passwordValidationResult = _passwordHasher.VerifyHashedPassword(
                request.User,
                request.User.Password,
                request.Password
            );

            if (passwordValidationResult == PasswordVerificationResult.Failed)
            {
                throw new ForbiddenException();
            }
        }

        private async Task AppendEvents(DeleteAccount request, CancellationToken cancellationToken)
        {
            var @event = new AccountDeleted(request.User.Id, request.User.Email);
            await _authTokenService.BlackListRefreshTokens(request.User.Id);

            foreach (var groupId in request.User.GroupIds)
            {
                await _groupService.LeaveGroup(groupId, request.User, cancellationToken);
            }

            await _userService.AppendAsync(request.User.Id, @event, cancellationToken);
        }
    }
}
