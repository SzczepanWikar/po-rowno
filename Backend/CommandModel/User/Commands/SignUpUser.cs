using System;
using System.Net.Mail;
using CommandModel.User.Services;
using Core.Common.Exceptions;
using Core.Common.Projections;
using Core.User;
using Infrastructure.Email.Service;
using Infrastructure.EventStore.Repository;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommandModel.User.Commands
{
    public record SignUpUser(
        string Username,
        string Email,
        string Password,
        string ReapetedPassword
    ) : IRequest<Guid>;

    public class SignUpUserHandler : IRequestHandler<SignUpUser, Guid>
    {
        private readonly IUserService _service;
        private readonly IEventStoreRepository<UserToken> _tokenRepository;
        private readonly IIndexProjectionRepository _indexedEmailRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public SignUpUserHandler(
            IUserService service,
            IEventStoreRepository<UserToken> tokenRepository,
            [FromKeyedServices(InternalProjectionName.EmailIndex)]
                IIndexProjectionRepository indexedEmailRepository,
            IEmailService emailService,
            IConfiguration configuration
        )
        {
            _service = service;
            _tokenRepository = tokenRepository;
            _indexedEmailRepository = indexedEmailRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<Guid> Handle(SignUpUser request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ReapetedPassword)
            {
                throw new BadRequestException("Passwords are not same.");
            }

            await _indexedEmailRepository.CheckAvailibility(request.Email, cancellationToken);

            var id = await _service.CreateAsync(request, cancellationToken);

            var token = await GenerateToken(id);

            await SendEmail(request.Username, request.Email, token);

            return id;
        }

        private async Task SendEmail(string username, string emailAddress, Guid token)
        {
            var to = new[] { new ReceiverData(username, emailAddress) };
            var url = _configuration.GetValue<string>("BaseUrl");

            var emailMessage = new EmailMessage(
                to,
                "Account Activation",
                $"To activate your account, visit the following link: {url}/AuthView/AccountActivation/{token}",
                MimeKit.Text.TextFormat.Text
            );
            await _emailService.SendEmailAsync(emailMessage);
        }

        private async Task<Guid> GenerateToken(Guid userId)
        {
            var id = Guid.NewGuid();
            var token = new UserTokenGenerated(id, userId, UserTokenType.AccountActivation);
            await _tokenRepository.CreateAsync(id, token);

            return id;
        }
    }
}
