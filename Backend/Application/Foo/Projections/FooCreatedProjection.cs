using Application.Foo.Projections;
using Core.Common;
using Core.Foo;
using MediatR;

namespace Application.Foo.Projections
{
    public class FooCreatedProjection : INotificationHandler<EventNotification<FooCreated>>
    {
        public Task Handle(EventNotification<FooCreated> notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("Test");
            return Task.CompletedTask;
        }
    }
}