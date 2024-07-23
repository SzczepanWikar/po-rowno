using Infrastructure.Email.Service;

namespace Infrastructure.Email.Store.Events
{
    public sealed record EmailSendingFailed(
        Guid Id,
        IEnumerable<ReceiverData> To,
        string Subject,
        string Content,
        MimeKit.Text.TextFormat TextFormat,
        string Error
    );
}
