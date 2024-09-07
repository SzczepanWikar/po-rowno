using Core.Common.Exceptions;
using Core.Common.Projections;
using Core.Group;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Group.Commands
{
    using Group = Core.Group.Group;
    using User = Core.User.User;

    public record JoinGroup(string code, User user) : IRequest;

    public class JoinGroupHandler : IRequestHandler<JoinGroup>
    {
        private readonly IGroupService _groupService;
        private readonly IEventStoreRepository<Group> _eventStoreRepository;
        private readonly IIndexProjectionRepository _indexProjectionRepository;

        public JoinGroupHandler(
            IGroupService groupService,
            IEventStoreRepository<Group> eventStoreRepository,
            [FromKeyedServices(InternalProjectionName.GroupCodeIndex)]
                IIndexProjectionRepository indexProjectionRepository
        )
        {
            _groupService = groupService;
            _eventStoreRepository = eventStoreRepository;
            _indexProjectionRepository = indexProjectionRepository;
        }

        public async Task Handle(JoinGroup request, CancellationToken cancellationToken)
        {
            Group group = await FindGroupByCode(request, cancellationToken);

            ValidateCode(request.code, group);

            var userId = request.user.Id;

            ValidateUser(group, userId);

            var @event = new UserJoinedGroup(group.Id, userId);

            await _eventStoreRepository.AppendAsync(group.Id, @event, cancellationToken);
        }

        private async Task<Group> FindGroupByCode(
            JoinGroup request,
            CancellationToken cancellationToken
        )
        {
            var groupId = await _indexProjectionRepository.GetOwnerId(
                request.code,
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
    }
}
