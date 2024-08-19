namespace Infrastructure.Projections.InternalProjections.Repository
{
    public interface IIndexProjectionRepository
    {
        Task CheckAvailibility(string indexedValue, CancellationToken cancellationToken = default);

        Task<Guid?> GetOwnerId(string indexedValue, CancellationToken cancellationToken = default);
    }
}
