using Application.Foo.Projections;
using Core.Common;
using Core.Foo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Foo.Projections
{
    public class FooCreatedProjection : INotificationHandler<EventNotification<FooCreated>>
    {
        public Task Handle(EventNotification<FooCreated> notification, CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }
}