using CommandModel.User.Services;
using Core.Common.Exceptions;
using Core.Common.Projections;
using Core.Group;
using Core.UserGroupEvents;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CommandModel.Group.Commands
{
    using Group = Core.Group.Group;
    using User = Core.User.User;

    public record JoinGroup(string Code, User User) : IRequest;

    public class JoinGroupHandler : IRequestHandler<JoinGroup>
    {
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IIndexProjectionRepository _indexProjectionRepository;

        public JoinGroupHandler(
            IGroupService groupService,
            IUserService userService,
            [FromKeyedServices(InternalProjectionName.GroupCodeIndex)]
                IIndexProjectionRepository indexProjectionRepository
        )
        {
            _groupService = groupService;
            _userService = userService;
            _indexProjectionRepository = indexProjectionRepository;
        }

        public async Task Handle(JoinGroup request, CancellationToken cancellationToken)
        {
            Group group = await FindGroupByCode(request, cancellationToken);

            ValidateCode(request.Code, group);

            var userId = request.User.Id;

            ValidateUser(group, userId);

            await AppenEvents(group, userId, cancellationToken);
        }

        private async Task<Group> FindGroupByCode(
            JoinGroup request,
            CancellationToken cancellationToken
        )
        {
            var groupId = await _indexProjectionRepository.GetOwnerId(
                request.Code,
                cancellationToken
            );

            if (!groupId.HasValue)
            {
                throw new NotFoundException("Group not found");
            }

            var group = await _groupService.FindOneAsync(groupId.Value, cancellationToken);
            return group;
        }

        private static void ValidateCode(string code, Group group)
        {
            var currentCode = group.Codes.Peek();

            if (!currentCode.Check(code, GroupCodeType.Join))
            {
                throw new BadRequestException("Incorrect code");
            }
        }

        private static void ValidateUser(Group group, Guid userId)
        {
            if (group.UsersIds.Any(e => e == userId))
            {
                throw new BadRequestException("User is already part of this group");
            }

            if (group.BannedUsersIds.Any(e => e == userId))
            {
                throw new BadRequestException("User is banned in this group.");
            }
        }

        private async Task AppenEvents(
            Group group,
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            var @event = new UserJoinedGroup(group.Id, userId);

            await _groupService.AppendAsync(group.Id, @event, cancellationToken);
            await _userService.AppendAsync(userId, @event, cancellationToken);
        }
    }
}
