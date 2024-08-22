namespace Application.Group
{
    using Group = Core.Group.Group;

    public interface IGroupService
    {
        Task<Group> FindOneAsync(Guid id, CancellationToken cancellationToken = default);
        Task Append(Guid id, object @event, CancellationToken cancellationToken = default);
    }
}
