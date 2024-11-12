using Core.Common.Exceptions;
using Core.Group.Events;
using Core.UserGroupEvents;
using MediatR;
using WriteModel.User.Services;

namespace WriteModel.Group.Commands
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
            await _groupService.LeaveGroup(request.Id, request.User, cancellationToken);
        }
    }
}
