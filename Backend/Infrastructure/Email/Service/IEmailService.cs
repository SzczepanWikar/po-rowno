namespace Infrastructure.Email.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
