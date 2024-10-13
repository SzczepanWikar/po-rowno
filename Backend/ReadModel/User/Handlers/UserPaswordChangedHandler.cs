using Core.Common.Projections;
using Core.User.Events;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.User.Handlers
{
    public sealed class UserPaswordChangedHandler : IEventNotificationHandler<UserPaswordChanged>
    {
        private readonly ApplicationContext _context;

        public UserPaswordChangedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<UserPaswordChanged> notification,
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

            user.Password = notification.Event.Password;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
