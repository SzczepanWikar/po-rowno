using Core.Common.Aggregate;

namespace Core.User
{
    public enum UserTokenType
    {
        AccountActivation = 0,
    }

    public sealed class UserToken : Aggregate
    {
        public Guid UserId { get; private set; }
        public bool Used { get; private set; } = false;
        public UserTokenType Type { get; private set; }

        public override void When(object @event)
        {
            switch (@event)
            {
                case UserTokenGenerated(Guid id, Guid userId, UserTokenType type):
                    Id = id;
                    UserId = userId;
                    Type = type;
                    break;
                case UserTokenUsed:
                    Used = true;
                    break;
                default:
                    return;
            }
        }
    }

    public sealed record UserTokenGenerated(Guid Id, Guid IdUser, UserTokenType Type);

    public sealed record UserTokenUsed(Guid Id);
}
