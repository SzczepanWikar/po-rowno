using Core.Common.Projections;
using Core.UserGroupEvents;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.UserGroup.Handlers
{
    public sealed class UserBannedFromGroupHandler : IEventNotificationHandler<UserBannedFromGroup>
    {
        private readonly ApplicationContext _context;

        public UserBannedFromGroupHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<UserBannedFromGroup> notification,
            CancellationToken cancellationToken
        )
        {
            var @event = notification.Event;
            var userGroup = await _context
                .Set<UserGroupEntity>()
                .Where(e =>
                    e.GroupId == @event.GroupId
                    && e.UserId == @event.UserId
                    && e.Status == UserGroupStatus.Active
                )
                .FirstOrDefaultAsync(cancellationToken);

            if (userGroup is null)
            {
                return;
            }

            userGroup.Status = UserGroupStatus.Banned;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
