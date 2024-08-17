namespace Core.Group.Events
{
    public record GroupCreated(
        Guid Id,
        string Name,
        string Description,
        Currency Currency,
        Guid OwnerId
    );
}
