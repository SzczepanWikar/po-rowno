using Core.Common.Projections;
using Core.Foo;
using Microsoft.EntityFrameworkCore;

namespace Application.Foo.Projections
{
    public class FooCreatedProjection : IEventNotificationHandler<FooCreated>
    {
        private readonly ApplicationContext _dbContext;

        public FooCreatedProjection(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(EventNotification<FooCreated> notification, CancellationToken cancellationToken)
        {   
            var existing = await _dbContext.Foos.Select(e => e.Id).Where(e => e == notification.@event.Id).FirstOrDefaultAsync();
            
            if (existing != Guid.Empty) {
                return;
            }

            var entity = new FooEntity
            {
                Id = notification.@event.Id,
                Name = notification.@event.Name,
                SomeNumber = notification.@event.SomeNumber,
            };

            await _dbContext.Foos.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}