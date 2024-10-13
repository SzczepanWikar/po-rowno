using Core.Common.Projections;
using Core.Group.Events;
using Microsoft.EntityFrameworkCore;
using ReadModel.User;

namespace ReadModel.Group.Handlers
{
    internal class GroupDataUpdatedHandler : IEventNotificationHandler<GroupDataUpdated>
    {
        private readonly ApplicationContext _context;

        public GroupDataUpdatedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<GroupDataUpdated> notification,
            CancellationToken cancellationToken
        )
        {
            var group = await _context
                .Set<GroupEntity>()
                .Where(e => e.Id == notification.Event.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (group is null)
            {
                return;
            }

            group.Name = notification.Event.Name ?? group.Name;
            group.Description = notification.Event.Description ?? group.Description;

            if (notification.Event.OwnerId.HasValue)
            {
                await SetOwner(notification.Event.OwnerId.Value, group, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SetOwner(
            Guid ownerId,
            GroupEntity group,
            CancellationToken cancellationToken
        )
        {
            var owner = await _context
                .Set<UserEntity>()
                .Where(e => e.Id == ownerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (owner is null)
            {
                return;
            }

            group.Owner = owner;
        }
    }
}
