using Core.Group;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace Application.Group.Commands
{
    using Group = Core.Group.Group;
    using User = Core.User.User;

    public record CreateGroup(User User, string Name, string Description, Currency Currency)
        : IRequest<Guid>;

    public class CreateGroupHandler : IRequestHandler<CreateGroup, Guid>
    {
        private readonly IEventStoreRepository<Group> _eventStoreRepository;

        public CreateGroupHandler(IEventStoreRepository<Group> eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public async Task<Guid> Handle(CreateGroup request, CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            var groupCreated = new GroupCreated(
                guid,
                request.Name,
                request.Description,
                request.Currency,
                request.User.Id
            );

            await _eventStoreRepository.Create(guid, groupCreated, cancellationToken);

            return guid;
        }
    }
}
