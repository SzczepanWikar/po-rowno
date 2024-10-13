using Core.Common.Projections;
using Core.Group.Events;
using Microsoft.EntityFrameworkCore;
using ReadModel.User;

namespace ReadModel.Group.Handlers
{
    public sealed class GroupOwnerChangedHandler : IEventNotificationHandler<GroupOwnerChanged>
    {
        public ApplicationContext _context;

        public GroupOwnerChangedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<GroupOwnerChanged> notification,
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

            var group = await _context
                .Set<GroupEntity>()
                .Where(e => e.Id == notification.Event.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (group is null)
            {
                return;
            }

            group.Owner = user;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
