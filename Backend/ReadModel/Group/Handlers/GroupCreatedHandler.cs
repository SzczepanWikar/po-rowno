using Core.Common.Projections;
using Core.Group.Events;
using Microsoft.EntityFrameworkCore;
using ReadModel.User;
using ReadModel.UserGroup;

namespace ReadModel.Group.Handlers
{
    public sealed class GroupCreatedHandler : IEventNotificationHandler<GroupCreated>
    {
        private readonly ApplicationContext _context;

        public GroupCreatedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<GroupCreated> notification,
            CancellationToken cancellationToken
        )
        {
            var owner = await _context
                .Set<UserEntity>()
                .Where(e => e.Id == notification.Event.OwnerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (owner is null)
            {
                return;
            }

            var userGroup = new UserGroupEntity
            {
                UserId = notification.Event.OwnerId,
                GroupId = notification.Event.Id,
                Status = UserGroupStatus.Active,
            };

            var group = new GroupEntity
            {
                Id = notification.Event.Id,
                Name = notification.Event.Name,
                Description = notification.Event.Description,
                Owner = owner,
                UserGroups = new List<UserGroupEntity> { userGroup },
                Currency = notification.Event.Currency,
            };

            await _context.AddAsync(group);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
