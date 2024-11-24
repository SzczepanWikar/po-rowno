using Core.Common.Exceptions;
using Core.UserGroupEvents;
using MediatR;

namespace CommandModel.Group.Commands
{
    public sealed record UnbanUser(Guid GroupId, Guid BannedUserId, Core.User.User user) : IRequest;

    public sealed class UnbanUserHandler : IRequestHandler<UnbanUser>
    {
        private readonly IGroupService _service;

        public UnbanUserHandler(IGroupService service)
        {
            _service = service;
        }

        public async Task Handle(UnbanUser request, CancellationToken cancellationToken)
        {
            var group = await _service.FindOneAsync(request.GroupId, cancellationToken);

            Validate(request, group);

            var @event = new UserUnbannedFromGroup(request.GroupId, request.BannedUserId);

            await _service.AppendAsync(request.GroupId, @event, cancellationToken);
        }

        private void Validate(UnbanUser request, Core.Group.Group group)
        {
            if (group.OwnerId != request.user.Id)
            {
                throw new ForbiddenException();
            }

            if (!group.BannedUsersIds.Any(e => e == request.BannedUserId))
            {
                throw new BadRequestException("User is not banned in this group.");
            }
        }
    }
}
