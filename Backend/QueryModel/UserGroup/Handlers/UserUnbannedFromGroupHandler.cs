using Core.Common.Projections;
using Core.UserGroupEvents;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.UserGroup.Handlers
{
    public sealed class UserUnbannedFromGroupHandler
        : IEventNotificationHandler<UserUnbannedFromGroup>
    {
        private readonly ApplicationContext _context;

        public UserUnbannedFromGroupHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<UserUnbannedFromGroup> notification,
            CancellationToken cancellationToken
        )
        {
            var @event = notification.Event;
            var userGroup = await _context
                .Set<UserGroupEntity>()
                .Where(e =>
                    e.GroupId == @event.GroupId
                    && e.UserId == @event.UserId
                    && e.Status == UserGroupStatus.Banned
                )
                .FirstOrDefaultAsync(cancellationToken);

            if (userGroup is null)
            {
                return;
            }

            userGroup.Status = UserGroupStatus.Active;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
