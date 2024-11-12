using Core.Common.Exceptions;
using Core.User.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WriteModel.User.Services;

namespace WriteModel.User.Commands
{
    using User = Core.User.User;

    public sealed record ChangeUserPassword(string OldPassword, string NewPassword, User User)
        : IRequest;

    public sealed class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPassword>
    {
        private readonly IUserService _userService;
        private readonly IAuthTokenService _authTokenService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ChangeUserPasswordHandler(
            IUserService userService,
            IAuthTokenService authTokenService,
            IPasswordHasher<User> passwordHasher
        )
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _authTokenService = authTokenService;
        }

        public async Task Handle(ChangeUserPassword request, CancellationToken cancellationToken)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(
                request.User,
                request.User.Password,
                request.OldPassword
            );

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Old password is incorrect.");
            }

            var hashedPassword = _passwordHasher.HashPassword(request.User, request.NewPassword);

            var @event = new UserPaswordChanged(request.User.Id, hashedPassword);
            await _userService.AppendAsync(request.User.Id, @event, cancellationToken);

            await _authTokenService.BlackListRefreshTokens(request.User.Id, cancellationToken);
        }
    }
}
