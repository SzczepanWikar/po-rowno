using Application.Group.Commands;

namespace Application.Group
{
    using Group = Core.Group.Group;

    public interface IGroupService
    {
        Task<Guid> CreateAsync(CreateGroup command, CancellationToken cancellationToken = default);
        Task<Group> FindOneAsync(Guid id, CancellationToken cancellationToken = default);
        Task AppendAsync(Guid id, object @event, CancellationToken cancellationToken = default);
    }
}
