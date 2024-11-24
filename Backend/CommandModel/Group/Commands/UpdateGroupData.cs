using CommandModel.User.Services;
using Core.Common.Exceptions;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace CommandModel.Group.Commands
{
    using Group = Core.Group.Group;
    using User = Core.User.User;

    public record UpdateGroupData(
        Guid Id,
        User user,
        string? Name,
        string? Description,
        Guid? OwnerId
    ) : IRequest;

    public class UpdateGroupDataHandler : IRequestHandler<UpdateGroupData>
    {
        private readonly IEventStoreRepository<Group> _eventStoreRepository;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;

        public UpdateGroupDataHandler(
            IEventStoreRepository<Group> eventStoreRepository,
            IGroupService groupService,
            IUserService userService
        )
        {
            _eventStoreRepository = eventStoreRepository;
            _groupService = groupService;
            _userService = userService;
        }

        public async Task Handle(UpdateGroupData request, CancellationToken cancellationToken)
        {
            var group = await _groupService.FindOneAsync(request.Id, cancellationToken);

            if (request.user.Id != group.OwnerId)
            {
                throw new ForbiddenException();
            }

            await CheckOwnerEdition(group, cancellationToken, request.OwnerId);

            var @event = new GroupDataUpdated(
                request.Id,
                request.Name,
                request.Description,
                request.OwnerId
            );

            await _eventStoreRepository.AppendAsync(request.Id, @event, cancellationToken);
        }

        private async Task CheckOwnerEdition(
            Group group,
            CancellationToken cancellationToken,
            Guid? ownerId
        )
        {
            if (!ownerId.HasValue)
            {
                return;
            }

            await _userService.FindOneAsync(ownerId.Value, cancellationToken);

            if (!group.UsersIds.Any(e => e == ownerId.Value))
            {
                throw new BadRequestException("Owner must be part of group");
            }
        }
    }
}
