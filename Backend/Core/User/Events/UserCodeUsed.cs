using Core.Common.Code;

namespace Core.User.Events
{
    public sealed record UserCodeUsed(Guid Id, string Code);
}
