using MediatR;

namespace Core.Common.Projections
{
    public record EventNotification<T>(T Event) : INotification;
}
