using MimeKit;

namespace Infrastructure.Email.Service
{
    public sealed class EmailMessage
    {
        public IEnumerable<ReceiverData> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public MimeKit.Text.TextFormat TextFormat { get; set; }

        public EmailMessage(
            IEnumerable<ReceiverData> to,
            string subject,
            string content,
            MimeKit.Text.TextFormat textFormat
        )
        {
            To = to;
            Subject = subject;
            Content = content;
            TextFormat = textFormat;
        }
    }

    public sealed class ReceiverData
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public ReceiverData(string name, string address)
        {
            Name = name;
            Address = address;
        }
    }
}
