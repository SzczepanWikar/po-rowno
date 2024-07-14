using Application.Foo.Projections;
using Core.Common;
using Core.Foo;
using MediatR;

namespace Application.Foo.Projections
{
    //public record FooCreatedNotification(FooCreated @event) : EventNotification<FooCreated>;

    public class FooCreatedProjection : INotificationHandler<EventNotification<FooCreated>>
    {
        public Task Handle(EventNotification<FooCreated> notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("Test");
            return Task.CompletedTask;
        }
    }

    public class FooUpdatedProjection : INotificationHandler<EventNotification<FooUpdated>>
    {
        public Task Handle(EventNotification<FooUpdated> notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("Test");
            return Task.CompletedTask;
        }
    }
}