using Application.Group.Commands;
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

        public async Task<Guid> CreateAsync(
            CreateGroup command,
            CancellationToken cancellationToken = default
        )
        {
            var id = Guid.NewGuid();
            var groupCreated = new GroupCreated(
                id,
                command.Name,
                command.Description,
                command.Currency,
                command.User.Id
            );

            await _repository.CreateAsync(id, groupCreated, cancellationToken);

            return id;
        }

        public async Task<Group> FindOneAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var group = await _repository.FindOneAsync(id, cancellationToken);

            if (group is null || group.Deleted)
            {
                throw new NotFoundException("Group not found!");
            }

            return group;
        }

        public async Task AppendAsync(
            Guid id,
            object @event,
            CancellationToken cancellationToken = default
        )
        {
            await _repository.AppendAsync(id, @event, cancellationToken);
        }
    }
}
