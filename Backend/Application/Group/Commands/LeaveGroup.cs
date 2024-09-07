using System.Threading;
using Core.Common.Exceptions;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace Application.Group.Commands
{
    using Group = Core.Group.Group;
    using User = Core.User.User;

    public sealed record LeaveGroup(Guid Id, User User) : IRequest;

    public sealed class LeaveGroupHandler : IRequestHandler<LeaveGroup>
    {
        private readonly IGroupService _groupService;

        public LeaveGroupHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task Handle(LeaveGroup request, CancellationToken cancellationToken)
        {
            var group = await _groupService.FindOneAsync(request.Id, cancellationToken);

            if (!group.UsersIds.Any(e => e == request.User.Id))
            {
                throw new BadRequestException("User is not part of this group.");
            }

            var @event = new UserLeavedGroup(request.Id, request.User.Id);
            await _groupService.AppendAsync(request.Id, @event, cancellationToken);

            if (group.OwnerId == request.User.Id)
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

            await _groupService.AppendAsync(group.Id, @event, cancellationToken);
        }
    }
}
