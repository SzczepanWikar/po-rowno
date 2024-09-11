namespace Application.User.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Application.User.Services;
    using MediatR;
    using User = Core.User.User;

    public sealed record Logout(User User) : IRequest;

    public sealed class LogoutHandler : IRequestHandler<Logout>
    {
        private readonly IUserService _userService;
        private readonly IAuthTokenService _authTokenService;

        public LogoutHandler(IUserService userService, IAuthTokenService authTokenService)
        {
            _userService = userService;
            _authTokenService = authTokenService;
        }

        public async Task Handle(Logout request, CancellationToken cancellationToken)
        {
            await _authTokenService.BlackListRefreshTokens(request.User.Id, cancellationToken);
        }
    }
}
