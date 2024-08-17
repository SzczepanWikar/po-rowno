namespace Application.Group
{
    public interface IGroupService
    {
        Task<Core.Group.Group> FindOneAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
