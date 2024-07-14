using Core.Foo;
using MediatR;

namespace Core.Common.Projections
{
    public interface IEventNotificationHandler<T>: INotificationHandler<EventNotification<T>>{}
}
