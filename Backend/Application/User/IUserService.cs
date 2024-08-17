namespace Application.User
{
    public interface IUserService
    {
        Task<Core.User.User> FindOneAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
