using Application.User.Auth;
using Core.Common.Code;
using Core.Common.Exceptions;
using Core.User;
using Core.User.Events;
using Core.User.UserToken;
using Infrastructure.Email.Service;
using Infrastructure.EventStore.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.User.Commands
{
    using User = Core.User.User;

    public record SignUpUser(
        string Username,
        string Email,
        string Password,
        string ReapetedPassword
    ) : IRequest<Guid>;

    public class SignUpUserHandler : IRequestHandler<SignUpUser, Guid>
    {
        private readonly IEventStoreRepository<User> _repository;
        private readonly IEventStoreRepository<UserToken> _tokenRepository;
        private readonly EmailConflictValidator _emailConflictValidator;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public SignUpUserHandler(
            IEventStoreRepository<User> repository,
            IEventStoreRepository<UserToken> tokenRepository,
            EmailConflictValidator emailConflictValidator,
            IEmailService emailService,
            IConfiguration configuration
        )
        {
            _repository = repository;
            _tokenRepository = tokenRepository;
            _emailConflictValidator = emailConflictValidator;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<Guid> Handle(SignUpUser request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ReapetedPassword)
            {
                throw new BadRequestException("Passwords are not same.");
            }

            await _emailConflictValidator.CheckAvailibility(request.Email, cancellationToken);

            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password, 10);

            var userSignedUp = new UserSignedUp(
                Guid.NewGuid(),
                request.Username,
                request.Email,
                hashedPassword
            );

            await _repository.Create(userSignedUp.Id, userSignedUp, cancellationToken);
            var token = await GenerateToken(userSignedUp.Id);

            await SendEmail(userSignedUp, token);

            return userSignedUp.Id;
        }

        private async Task SendEmail(UserSignedUp userSignedUp, Guid token)
        {
            var to = new[] { new ReceiverData(userSignedUp.Username, userSignedUp.Email) };
            var url = _configuration.GetValue<string>("BaseUrl");

            var email = new EmailMessage(
                to,
                "Aktywacja konta",
                $"Aby aktywować konto odwiedź podany link: {url}/AuthView/AccountActivation/{token}",
                MimeKit.Text.TextFormat.Text
            );
            await _emailService.SendEmailAsync(email);
        }

        private async Task<Guid> GenerateToken(Guid userId)
        {
            var id = Guid.NewGuid();
            var token = new UserTokenGenerated(id, userId);
            await _tokenRepository.Create(id, token);

            return id;
        }
    }
}
