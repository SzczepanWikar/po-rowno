using Core.Group;
using MediatR;

namespace WriteModel.Group.Commands
{
    public record CreateGroup(
        Core.User.User User,
        string Name,
        string Description,
        Currency Currency
    ) : IRequest<Guid>;

    public class CreateGroupHandler : IRequestHandler<CreateGroup, Guid>
    {
        private readonly IGroupService _service;

        public CreateGroupHandler(IGroupService service)
        {
            _service = service;
        }

        public async Task<Guid> Handle(CreateGroup request, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(request, cancellationToken);
        }
    }
}
