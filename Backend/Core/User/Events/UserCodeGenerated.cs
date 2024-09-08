using Core.Common.Code;

namespace Core.User.Events
{
    public sealed record UserCodeGenerated(Guid Id, Code<UserCodeType> Code);
}
