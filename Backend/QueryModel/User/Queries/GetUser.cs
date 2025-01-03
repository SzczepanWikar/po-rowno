﻿using Core.Common.Exceptions;
using Core.ProjectionEntities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace QueryModel.User.Queries
{
    public sealed record GetUser(Core.User.User User) : IRequest<UserEntity>;

    public sealed class GetUserHandler : IRequestHandler<GetUser, UserEntity>
    {
        private readonly ApplicationContext _context;

        public GetUserHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> Handle(GetUser request, CancellationToken cancellationToken)
        {
            var user = await _context
                .Set<UserEntity>()
                .Where(u => u.Id == request.User.Id && u.Deleted == false)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            return user;
        }
    }
}
