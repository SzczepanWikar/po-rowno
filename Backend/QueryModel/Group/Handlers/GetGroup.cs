using Core.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryModel.UserGroup;

namespace QueryModel.Group.Handlers
{
    public sealed record GetGroup(Guid Id, Core.User.User User) : IRequest<GroupEntity>;

    public sealed class GetGroupHandler : IRequestHandler<GetGroup, GroupEntity>
    {
        private readonly ApplicationContext _context;

        public GetGroupHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<GroupEntity> Handle(GetGroup request, CancellationToken cancellationToken)
        {
            var userId = request.User.Id;
            var group = await _context
                .Set<GroupEntity>()
                .Include(g => g.UserGroups.Where(ug => ug.Status != UserGroupStatus.Leaved))
                .ThenInclude(ug => ug.User)
                .Include(g => g.Balances.Where(b => b.Balance != 0))
                .Include(g => g.Owner)
                .Where(g =>
                    g.Id == request.Id
                    && g.UserGroups.Any(ug =>
                        ug.UserId == userId && ug.Status == UserGroupStatus.Active
                    )
                )
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (group is null)
            {
                throw new NotFoundException("Group no found!");
            }

            return group;
        }
    }
}
