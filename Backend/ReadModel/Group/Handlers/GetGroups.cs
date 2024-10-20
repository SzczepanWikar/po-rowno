﻿using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.Group.Handlers
{
    public sealed record GetGroups(Core.User.User User) : IRequest<IEnumerable<GroupEntity>>;

    public sealed class GetGroupsHandler : IRequestHandler<GetGroups, IEnumerable<GroupEntity>>
    {
        private readonly ApplicationContext _context;

        public GetGroupsHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GroupEntity>> Handle(
            GetGroups request,
            CancellationToken cancellationToken
        )
        {
            var userId = request.User.Id;
            var groups = await _context
                .Set<GroupEntity>()
                .Select(g => new GroupEntity
                {
                    Id = g.Id,
                    Currency = g.Currency,
                    Description = g.Description,
                    JoinCode = g.JoinCode,
                    JoinCodeValidTo = g.JoinCodeValidTo,
                    Name = g.Name,
                    OwnerId = g.OwnerId,
                    Balances = g
                        .Balances.Where(b =>
                            b.Balance != 0 && (b.PayerId == userId || b.DeptorId == userId)
                        )
                        .ToList(),
                })
                .AsNoTracking()
                .ToListAsync();

            return groups;
        }
    }
}
