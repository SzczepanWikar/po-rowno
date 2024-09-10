using Application.User.Services;
using Core.Common.Code;
using Core.Common.Exceptions;
using Core.Common.Projections;
using Core.User;
using Core.User.Events;
using Infrastructure.Email.Service;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.User.Commands
{
    public sealed record RequestPasswordReset(string Email) : IRequest;

    public sealed class RequestPasswordResetHandler : IRequestHandler<RequestPasswordReset>
    {
        private readonly IUserService _userService;
        private readonly IIndexProjectionRepository _indexedEmailRepository;
        private readonly IIndexProjectionRepository _indexedCodeRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public RequestPasswordResetHandler(
            [FromKeyedServices(InternalProjectionName.EmailIndex)]
                IIndexProjectionRepository indexedEmailRepository,
            [FromKeyedServices(InternalProjectionName.UserCodeIndex)]
                IIndexProjectionRepository indexedCodeRepository,
            IUserService userService,
            IConfiguration configuration,
            IEmailService emailService
        )
        {
            _indexedEmailRepository = indexedEmailRepository;
            _indexedCodeRepository = indexedCodeRepository;
            _userService = userService;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task Handle(RequestPasswordReset request, CancellationToken cancellationToken)
        {
            var userId = await _indexedEmailRepository.GetOwnerId(request.Email);

            if (!userId.HasValue)
            {
                return;
            }

            var user = await _userService.FindOneAsync(userId.Value, cancellationToken);

            var code = await GenerateCode(cancellationToken);
            var @event = new UserCodeGenerated(userId.Value, code);
            await _userService.AppendAsync(userId.Value, @event, cancellationToken);

            await SendEmail(user, code);
        }

        private async Task SendEmail(Core.User.User user, Code<UserCodeType> code)
        {
            var to = new[] { new ReceiverData(user.Username, user.Email) };
            var url = _configuration.GetValue<string>("BaseUrl");

            var emailMessage = new EmailMessage(
                to,
                "Reset hasła",
                $"Aby przywrócić hasło wpisz podany kod do formularzu zmiany hasła: {code.Value}",
                MimeKit.Text.TextFormat.Text
            );
            await _emailService.SendEmailAsync(emailMessage);
        }

        private async Task<Code<UserCodeType>> GenerateCode(CancellationToken cancellationToken)
        {
            var attempts = 0;
            do
            {
                try
                {
                    if (attempts > 500)
                    {
                        throw new InvalidOperationException(
                            "Unable to generate a unique code after several attempts."
                        );
                    }

                    var code = new Code<UserCodeType>(
                        UserCodeType.ResetPassword,
                        DateTime.Now.AddMinutes(20),
                        true
                    );

                    await _indexedCodeRepository.CheckAvailibility(code.Value, cancellationToken);
                    return code;
                }
                catch (ConflictException ex)
                {
                    attempts++;
                }
            } while (true);
        }
    }
}
