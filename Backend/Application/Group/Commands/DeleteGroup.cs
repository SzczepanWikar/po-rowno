using Core.Common;
using Core.Common.Exceptions;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace Application.Group.Commands
{
    using Group = Core.Group.Group;

    public sealed record DeleteGroup(Guid Id, Core.User.User User) : IRequest;

    public sealed class DeleteGroupHandler : IRequestHandler<DeleteGroup>
    {
        private readonly IEventStoreRepository<Group> _repository;

        public DeleteGroupHandler(IEventStoreRepository<Group> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteGroup request, CancellationToken cancellationToken)
        {
            var group = await _repository.FindOneAsync(request.Id);

            if (group is null)
            {
                return;
            }

            if (group.OwnerId != request.User.Id)
            {
                throw new ForbiddenException();
            }

            await _repository.AppendAsync(request.Id, new Deleted(), cancellationToken);
        }
    }
}
