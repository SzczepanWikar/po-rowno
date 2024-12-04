using Core.Common.Projections;
using Core.ProjectionEntities;
using Core.UserGroupEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace QueryModel.UserGroup.Handlers
{
    public sealed class UserJoinedGroupHandler : IEventNotificationHandler<UserJoinedGroup>
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<UserJoinedGroupHandler> _logger;

        public UserJoinedGroupHandler(
            ApplicationContext context,
            ILogger<UserJoinedGroupHandler> logger
        )
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handle(
            EventNotification<UserJoinedGroup> notification,
            CancellationToken cancellationToken
        )
        {
            var @event = notification.Event;

            var validationResult = await ValidateDbState(@event, cancellationToken);

            if (!validationResult)
            {
                return;
            }

            UserGroupEntity? userGroup;

            userGroup = await _context
                .Set<UserGroupEntity>()
                .Where(e =>
                    e.UserId == @event.UserId
                    && e.GroupId == @event.GroupId
                    && e.Status == UserGroupStatus.Leaved
                )
                .FirstOrDefaultAsync();

            if (userGroup is null)
            {
                userGroup = new UserGroupEntity
                {
                    UserId = @event.UserId,
                    GroupId = @event.GroupId,
                    Status = UserGroupStatus.Active,
                };

                await _context.AddAsync(userGroup, cancellationToken);
            }
            else
            {
                userGroup.Status = UserGroupStatus.Active;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> ValidateDbState(
            UserJoinedGroup @event,
            CancellationToken cancellationToken
        )
        {
            var groupExists = await _context
                .Set<GroupEntity>()
                .Where(e => e.Id == @event.GroupId)
                .AnyAsync(cancellationToken);

            if (!groupExists)
            {
                _logger.LogWarning($"Group with index: {@event.GroupId} does not exists!.");

                return false;
            }

            var userExists = await _context
                .Set<UserEntity>()
                .Where(e => e.Id == @event.UserId && e.Deleted == false)
                .AnyAsync(cancellationToken);

            if (!userExists)
            {
                _logger.LogWarning($"User with index: {@event.UserId} does not exists!.");
                return false;
            }

            var alreadyExists = await _context
                .Set<UserGroupEntity>()
                .Where(e =>
                    e.UserId == @event.UserId
                    && e.GroupId == @event.GroupId
                    && e.Status != UserGroupStatus.Leaved
                )
                .AnyAsync(cancellationToken);

            if (alreadyExists)
            {
                _logger.LogWarning(
                    $"UserGroup with UserId: {@event.UserId} and GroupId: {@event.GroupId} already exists."
                );
                return false;
            }

            return true;
        }
    }
}
