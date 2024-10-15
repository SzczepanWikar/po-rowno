using Core.Common.Projections;
using Core.UserGroupEvents;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.UserGroup.Handlers
{
    public sealed class UserLeavedGroupHandler : IEventNotificationHandler<UserLeavedGroup>
    {
        private readonly ApplicationContext _context;

        public UserLeavedGroupHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<UserLeavedGroup> notification,
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

            userGroup.Status = UserGroupStatus.Leaved;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
