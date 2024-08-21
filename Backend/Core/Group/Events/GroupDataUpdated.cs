namespace Core.Group.Events
{
    public record GroupDataUpdated(Guid Id, string? Name, string? Description, Guid? OwnerId);
}
