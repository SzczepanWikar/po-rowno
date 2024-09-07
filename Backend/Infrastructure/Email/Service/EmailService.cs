using Infrastructure.Email.Store;
using Infrastructure.Email.Store.Events;
using Infrastructure.EventStore.Repository;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Email.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        private readonly ILogger<IEmailService> _logger;
        private readonly IEventStoreRepository<EmailMessageAggregate> _eventStoreRepository;

        public EmailService(
            EmailConfig emailConfig,
            ILogger<IEmailService> logger,
            IEventStoreRepository<EmailMessageAggregate> eventStoreRepository
        )
        {
            _emailConfig = emailConfig;
            _logger = logger;
            _eventStoreRepository = eventStoreRepository;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var mimeMessage = CreateEmailMessage(message);

            try
            {
                await SendAsync(mimeMessage);
                await StoreEmailMessage(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured during email sending.");
                _logger.LogError(ex.Message);
                await StoreEmailMessage(message, ex.Message);
            }
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(
                new MailboxAddress(_emailConfig.From.Name, _emailConfig.From.Address)
            );
            emailMessage.To.AddRange(message.To.Select(e => new MailboxAddress(e.Name, e.Address)));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(message.TextFormat) { Text = message.Content };

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.Host, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.Login, _emailConfig.Password);
                    await client.SendAsync(mailMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private async Task StoreEmailMessage(EmailMessage message, string? errorMessage = null)
        {
            object @event;
            Guid id = Guid.NewGuid();

            if (errorMessage == null)
            {
                @event = new EmailSent(
                    id,
                    message.To,
                    message.Subject,
                    message.Content,
                    message.TextFormat
                );
            }
            else
            {
                @event = new EmailSendingFailed(
                    id,
                    message.To,
                    message.Subject,
                    message.Content,
                    message.TextFormat,
                    errorMessage
                );
            }

            await _eventStoreRepository.CreateAsync(id, @event);
        }
    }
}
