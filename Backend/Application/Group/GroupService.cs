using Core.Common.Exceptions;
using Infrastructure.EventStore.Repository;

namespace Application.Group
{
    public class GroupService : IGroupService
    {
        private readonly IEventStoreRepository<Core.Group.Group> _repository;

        public GroupService(IEventStoreRepository<Core.Group.Group> repository)
        {
            _repository = repository;
        }

        public async Task<Core.Group.Group> FindOneAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var group = await _repository.Find(id, cancellationToken);

            if (group == null)
            {
                throw new NotFoundException("Group not found!");
            }

            return group;
        }
    }
}
