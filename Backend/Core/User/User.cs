using Core.Common.Aggregate;
using Core.Common.Code;

namespace Core.User
{
    public enum UserStatus
    {
        Inactive,
        Active,
        Blocked
    }

    public enum UserCodeType
    {
        AccountActivation,
        PasswordReset
    }

    public class User : Aggregate
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public UserStatus Status { get; private set; }
        public IEnumerable<Code<UserStatus>> Codes { get; private init; } =
            new List<Code<UserStatus>>();

        public override void When(object @event)
        {
            throw new NotImplementedException();
        }
    }
}
