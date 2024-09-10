using Core.Common.Projections;
using EventStore.Client;
using Infrastructure.Projections.InternalProjections.Repository;
using Microsoft.Extensions.Logging;

namespace Application.User.Repositories
{
    public class UserCodeIndexRepository : IndexProjectionRepository
    {
        protected override string StreamName { get; init; } =
            $"{InternalProjectionName.UserCodeIndex}-res";
        protected override string EmailIndexedEvent { get; init; } = "UserCodeIndexed";

        public UserCodeIndexRepository(
            EventStoreClient eventStoreClient,
            ILogger<IndexProjectionRepository> logger
        )
            : base(eventStoreClient, logger) { }
    }
}
