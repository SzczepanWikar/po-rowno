using Core.Common;
using Core.Common.Aggregate;
using Core.Common.Code;
using Core.User.Events;
using Core.UserGroupEvents;

namespace Core.User
{
    public enum UserStatus
    {
        Inactive,
        Active,
        Blocked,
    }

    public enum UserCodeType
    {
        ResetPassword,
    }

    public sealed class User : Aggregate
    {
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public UserStatus Status { get; private set; }
        public IList<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
        public Codes<UserCodeType> Codes { get; private set; } = new();
        public IList<Guid> GroupIds { get; private set; } = new List<Guid>();

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
                case UserSignedIn e:
                    RefreshTokens.Add(e.RefreshToken);
                    break;
                case UserPaswordChanged(_, string password):
                    Password = password;
                    break;
                case UserCodeGenerated(_, Code<UserCodeType> code):
                    Codes.Push(code);
                    break;
                case UserCodeUsed(_, string code):
                    MarkCodeAsUsed(code);
                    break;
                case RefreshTokenExpirationDateChanged(_, string token, DateTime date):
                    ChangeRefreshTokenExpirationDate(token, date);
                    break;
                case UserJoinedGroup(Guid groupId, _):
                    GroupIds.Add(groupId);
                    break;
                case UserLeavedGroup(Guid groupId, _):
                    GroupIds.Remove(groupId);
                    break;
                case UserBannedFromGroup(Guid groupId, _):
                    GroupIds.Remove(groupId);
                    break;
                case UserUnbannedFromGroup(Guid groupId, _):
                    GroupIds.Add(groupId);
                    break;
                case AccountDeleted e:
                    Deleted = true;
                    break;
                default:
                    return;
            }
        }

        private void MarkCodeAsUsed(string code)
        {
            var existing = Codes.FirstOrDefault(e => e.Value == code);

            if (existing == null)
            {
                return;
            }

            existing.Used = true;
        }

        private void ChangeRefreshTokenExpirationDate(string token, DateTime date)
        {
            var refreshToken = RefreshTokens.LastOrDefault(e => e.Token == token);

            if (refreshToken is null)
            {
                return;
            }

            refreshToken.ExpirationDate = date;
        }
    }
}
