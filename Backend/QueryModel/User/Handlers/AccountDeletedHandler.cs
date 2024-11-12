using Core.Common.Projections;
using Core.User.Events;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.User.Handlers
{
    public sealed class AccountDeletedHandler : IEventNotificationHandler<AccountDeleted>
    {
        private readonly ApplicationContext _context;

        public AccountDeletedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<AccountDeleted> notification,
            CancellationToken cancellationToken
        )
        {
            var user = await _context
                .Set<UserEntity>()
                .Where(e => e.Id == notification.Event.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return;
            }

            user.Delete();

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
