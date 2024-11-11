using Core.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.User.Handlers
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
                .Where(u => u.Id == request.User.Id)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            return user;
        }
    }
}
