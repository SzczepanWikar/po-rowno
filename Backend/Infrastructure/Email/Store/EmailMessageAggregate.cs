using Core.Common.Aggregate;
using Infrastructure.Email.Service;
using Infrastructure.Email.Store.Events;
using MimeKit;

namespace Infrastructure.Email.Store
{
    public sealed class EmailMessageAggregate : Aggregate
    {
        public IEnumerable<ReceiverData> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public MimeKit.Text.TextFormat TextFormat { get; set; }
        public bool Sent { get; set; }
        public string Error { get; set; }

        public override void When(object @event)
        {
            switch (@event)
            {
                case EmailSent(
                    Guid id,
                    IEnumerable<ReceiverData> to,
                    string subject,
                    string content,
                    MimeKit.Text.TextFormat textFormat
                ):
                    Id = id;
                    To = to;
                    Subject = subject;
                    Content = content;
                    TextFormat = textFormat;
                    Sent = true;
                    break;
                case EmailSendingFailed(
                    Guid id,
                    IEnumerable<ReceiverData> to,
                    string subject,
                    string content,
                    MimeKit.Text.TextFormat textFormat,
                    string error
                ):
                    Id = id;
                    To = to;
                    Subject = subject;
                    Content = content;
                    TextFormat = textFormat;
                    Error = error;
                    Sent = false;
                    break;
            }
        }
    }
}
