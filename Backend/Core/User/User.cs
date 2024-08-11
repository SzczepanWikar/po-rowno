using System.Linq;
using Core.Common.Aggregate;
using Core.Common.Code;
using Core.User.Events;

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

    public sealed class User : Aggregate
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public UserStatus Status { get; private set; }
        public string Token { get; private set; }

        public override void When(object @event)
        {
            switch (@event)
            {
                case UserSignedUp e:
                    Id = e.Id;
                    Username = e.Username;
                    Email = e.Email;
                    Password = e.Password;
                    Status = UserStatus.Inactive;
                    break;
                case AccountActivated:
                    Status = UserStatus.Active;
                    break;
                case UserSignedIn(_, string token):
                    Token = token;
                    break;
                default:
                    return;
            }
        }
    }
}
