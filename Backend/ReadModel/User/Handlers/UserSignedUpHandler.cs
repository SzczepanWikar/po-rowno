using Core.Common.Projections;
using Core.User;
using Core.User.Events;

namespace ReadModel.User.Handlers
{
    public sealed class UserSignedUpHandler : IEventNotificationHandler<UserSignedUp>
    {
        private readonly ApplicationContext _context;

        public UserSignedUpHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<UserSignedUp> notification,
            CancellationToken cancellationToken
        )
        {
            var user = new UserEntity
            {
                Id = notification.Event.Id,
                Username = notification.Event.Username,
                Email = notification.Event.Email,
                Status = UserStatus.Inactive,
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
