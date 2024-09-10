using Core.Common.Projections;
using EventStore.Client;
using Infrastructure.Projections.InternalProjections.Repository;
using Microsoft.Extensions.Logging;

namespace Application.User.Repositories
{
    public sealed class UserRefreshTokenIndexRepository : IndexProjectionRepository
    {
        protected override string StreamName { get; init; } =
            $"{InternalProjectionName.UserRefreshTokenIndex}-res";
        protected override string EmailIndexedEvent { get; init; } = "UserRefreshTokenIndexed";

        public UserRefreshTokenIndexRepository(
            EventStoreClient eventStoreClient,
            ILogger<IndexProjectionRepository> logger
        )
            : base(eventStoreClient, logger) { }
    }
}
