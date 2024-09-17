namespace WriteModel.User.Services
{
    using Core.User.Events;
    using WriteModel.User.Commands;
    using User = Core.User.User;

    public interface IUserService
    {
        Task<Guid> CreateAsync(SignUpUser command, CancellationToken cancellationToken = default);
        Task<User> FindOneAsync(Guid id, CancellationToken cancellationToken = default);
        Task AppendAsync(Guid id, object @event, CancellationToken cancellationToken = default);
        Task AppendAsync(
            Guid id,
            IReadOnlyCollection<object> @event,
            CancellationToken cancellationToken = default
        );
    }
}
