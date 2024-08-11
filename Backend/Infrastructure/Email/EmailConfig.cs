namespace Infrastructure.Email
{
    public sealed record EmailConfig
    {
        public EmailFromConfig From { get; init; }
        public string SenderEmail { get; init; }
        public string Host { get; init; }
        public int Port { get; init; }
        public string Login { get; init; }
        public string Password { get; init; }

        public sealed record EmailFromConfig
        {
            public string Name { get; init; }
            public string Address { get; init; }
        }
    }
}
