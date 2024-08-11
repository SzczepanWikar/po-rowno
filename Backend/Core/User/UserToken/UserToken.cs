using Core.Common.Aggregate;

namespace Core.User.UserToken
{
    public sealed class UserToken : Aggregate
    {
        public Guid UserId { get; set; }
        public bool Used { get; set; } = false;

        public override void When(object @event)
        {
            switch (@event)
            {
                case UserTokenGenerated(Guid id, Guid userId):
                    Id = id;
                    UserId = userId;
                    break;
                case UserTokenUsed:
                    Used = true;
                    break;
                default:
                    return;
            }
        }
    }

    public sealed record UserTokenGenerated(Guid Id, Guid IdUser);

    public sealed record UserTokenUsed(Guid Id);
}
