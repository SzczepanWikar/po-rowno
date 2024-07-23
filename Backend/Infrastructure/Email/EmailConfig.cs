namespace Infrastructure.Email
{
    public sealed class EmailConfig
    {
        public EmailFromConfig From { get; set; }
        public string SenderEmail { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public class EmailFromConfig
        {
            public string Name { get; set; }
            public string Address { get; set; }
        }
    }
}
