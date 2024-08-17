using Core.Common.Code;
using Core.Common.Exceptions;
using Core.Group;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace Application.Group.Commands
{
    public record GenerateJoinGroupCode(DateTime ValidTo, Guid GroupId, Core.User.User User)
        : IRequest;

    public class GenerateJoinGroupCodeHandler : IRequestHandler<GenerateJoinGroupCode>
    {
        private readonly IGroupService _groupService;
        private readonly IEventStoreRepository<Core.Group.Group> _repository;

        public GenerateJoinGroupCodeHandler(
            IGroupService groupService,
            IEventStoreRepository<Core.Group.Group> repository
        )
        {
            _groupService = groupService;
            _repository = repository;
        }

        public async Task Handle(GenerateJoinGroupCode request, CancellationToken cancellationToken)
        {
            var group = await _groupService.FindOneAsync(request.GroupId, cancellationToken);

            if (!group.UsersIds.Contains(request.User.Id))
            {
                throw new ForbiddenException();
            }

            var code = new Code<GroupCodeType>(GroupCodeType.Join, request.ValidTo);

            var @event = new GroupCodeGenerated(code);

            await _repository.Append(group.Id, @event, cancellationToken);
        }
    }
}
