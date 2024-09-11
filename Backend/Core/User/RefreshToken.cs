namespace Core.User
{
    public sealed class RefreshToken
    {
        public RefreshToken(string token, DateTime expirationDate)
        {
            Token = token;
            ExpirationDate = expirationDate;
        }

        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
