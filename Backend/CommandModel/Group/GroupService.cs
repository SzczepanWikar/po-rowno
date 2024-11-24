using CommandModel.Group.Commands;
using CommandModel.User.Services;
using Core.Common.Exceptions;
using Core.Group;
using Core.Group.Events;
using Core.UserGroupEvents;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.EventStore.Repository;

namespace CommandModel.Group
{
    using Group = Core.Group.Group;

    public class GroupService : IGroupService
    {
        private readonly IEventStoreRepository<Group> _repository;
        private readonly IUserService _userService;

        public GroupService(IEventStoreRepository<Group> repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }

        public async Task<Guid> CreateAsync(
            CreateGroup command,
            CancellationToken cancellationToken = default
        )
        {
            if (!Currency.IsDefined(command.Currency))
            {
                throw new BadRequestException("Incorrect currency!");
            }

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

        public async Task LeaveGroup(
            Guid id,
            Core.User.User user,
            CancellationToken cancellationToken = default
        )
        {
            var group = await FindOneAsync(id, cancellationToken);

            if (!group.UsersIds.Any(e => e == user.Id))
            {
                throw new BadRequestException("User is not part of this group.");
            }

            await AppendLeaveGroupEvents(user, group, cancellationToken);
        }

        private async Task AppendLeaveGroupEvents(
            Core.User.User user,
            Group group,
            CancellationToken cancellationToken
        )
        {
            var @event = new UserLeavedGroup(group.Id, user.Id);
            await AppendAsync(group.Id, @event, cancellationToken);
            await _userService.AppendAsync(user.Id, @event, cancellationToken);

            if (group.OwnerId == user.Id)
            {
                await ChangeGroupOwner(group, cancellationToken);
            }
        }

        private async Task ChangeGroupOwner(
            Group group,
            CancellationToken cancellationToken = default
        )
        {
            var userId = group.UsersIds.FirstOrDefault();

            var @event = new GroupOwnerChanged(group.Id, userId);

            await AppendAsync(group.Id, @event, cancellationToken);
        }
    }
}
