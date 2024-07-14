using Application.Foo.Projections;
using Core.Common;
using Core.Foo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Foo.Projections
{
    public class FooCreatedProjection : INotificationHandler<EventNotification<FooCreated>>
    {
        private readonly ApplicationContext _dbContext;

        public FooCreatedProjection(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(EventNotification<FooCreated> notification, CancellationToken cancellationToken)
        {
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