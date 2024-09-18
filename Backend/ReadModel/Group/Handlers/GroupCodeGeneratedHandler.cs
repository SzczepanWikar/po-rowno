using Core.Common.Projections;
using Core.Group.Events;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.Group.Handlers
{
    public sealed class GroupCodeGeneratedHandler : IEventNotificationHandler<GroupCodeGenerated>
    {
        private readonly ApplicationContext _context;

        public GroupCodeGeneratedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(EventNotification<GroupCodeGenerated> notification, CancellationToken cancellationToken)
        {
            var group = await _context.Set<GroupEntity>().Where(e => e.Id == notification.Event.GroupId).FirstOrDefaultAsync(cancellationToken);

            if (group is null)
            {
                return;
            }

            group.JoinCode = notification.Event.Code.Value;
            group.JoinCodeValidTo = notification.Event.Code.ValidTo;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
