using Core.Common.Exceptions;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;

namespace Application.Group
{
    using Group = Core.Group.Group;

    public class GroupService : IGroupService
    {
        private readonly IEventStoreRepository<Group> _repository;

        public GroupService(IEventStoreRepository<Group> repository)
        {
            _repository = repository;
        }

        public async Task<Group> FindOneAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var group = await _repository.Find(id, cancellationToken);

            if (group is null)
            {
                throw new NotFoundException("Group not found!");
            }

            return group;
        }

        public async Task Append(
            Guid id,
            object @event,
            CancellationToken cancellationToken = default
        )
        {
            await _repository.Append(id, @event, cancellationToken);
        }
    }
}
