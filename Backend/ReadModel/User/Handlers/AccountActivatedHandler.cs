using Core.Common.Projections;
using Core.User;
using Core.User.Events;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.User.Handlers
{
    public sealed class AccountActivatedHandler : IEventNotificationHandler<AccountActivated>
    {
        private readonly ApplicationContext _context;

        public AccountActivatedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<AccountActivated> notification,
            CancellationToken cancellationToken
        )
        {
            var user = await _context
                .Set<UserEntity>()
                .Where(e => e.Id == notification.Event.Id)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return;
            }

            user.Status = UserStatus.Active;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
