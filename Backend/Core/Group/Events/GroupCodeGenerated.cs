using Core.Common.Code;

namespace Core.Group.Events
{
    public record GroupCodeGenerated(Guid GroupId, Code<GroupCodeType> Code);
}
