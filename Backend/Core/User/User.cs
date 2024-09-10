﻿using Core.Common.Aggregate;
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
        ResetPassword
    }

    public sealed class User : Aggregate
    {
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public UserStatus Status { get; private set; }
        public IList<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
        public Codes<UserCodeType> Codes { get; private set; } = new();

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
    }
}
