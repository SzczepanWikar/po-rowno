using Core.Common.Exceptions;
using Core.UserGroupEvents;
using MediatR;

namespace CommandModel.Group.Commands
{
    public sealed record BanUser(Guid GroupId, Guid BannedUserId, Core.User.User user) : IRequest;

    public sealed class BanUserHandler : IRequestHandler<BanUser>
    {
        private readonly IGroupService _service;

        public BanUserHandler(IGroupService service)
        {
            _service = service;
        }

        public async Task Handle(BanUser request, CancellationToken cancellationToken)
        {
            var group = await _service.FindOneAsync(request.GroupId, cancellationToken);

            Validate(request, group);

            var @event = new UserBannedFromGroup(request.GroupId, request.BannedUserId);

            await _service.AppendAsync(request.GroupId, @event, cancellationToken);
        }

        private static void Validate(BanUser request, Core.Group.Group group)
        {
            if (group.OwnerId != request.user.Id)
            {
                throw new ForbiddenException();
            }

            if (!group.UsersIds.Any(e => request.BannedUserId == e))
            {
                throw new BadRequestException("User is not part of this group.");
            }

            if (group.BannedUsersIds.Any(e => e == request.BannedUserId))
            {
                throw new BadRequestException("User is already banned in this group.");
            }
        }
    }
}
