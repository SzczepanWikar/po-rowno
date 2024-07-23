using Infrastructure.Email.Service;
using MimeKit;

namespace Infrastructure.Email.Store.Events
{
    public sealed record EmailSent(
        Guid Id,
        IEnumerable<ReceiverData> To,
        string Subject,
        string Content,
        MimeKit.Text.TextFormat TextFormat
    );
}
