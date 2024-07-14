using MediatR;

namespace Core.Common
{
    public record EventNotification<T>(T @event) : INotification;
}