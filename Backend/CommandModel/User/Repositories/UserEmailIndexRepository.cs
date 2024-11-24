using Core.Common.Projections;
using EventStore.Client;
using Infrastructure.Projections.InternalProjections.Repository;
using Microsoft.Extensions.Logging;

namespace CommandModel.User.Repositories
{
    internal class UserEmailIndexRepository : IndexProjectionRepository
    {
        public UserEmailIndexRepository(
            EventStoreClient eventStoreClient,
            ILogger<IndexProjectionRepository> logger
        )
            : base(eventStoreClient, logger) { }

        protected override string StreamName { get; init; } =
            $"{InternalProjectionName.EmailIndex}-res";
        protected override string EmailIndexedEvent { get; init; } = "UserEmailIndexed";
    }
}
