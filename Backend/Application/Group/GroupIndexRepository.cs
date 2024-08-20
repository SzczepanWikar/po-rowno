using Core.Common.Projections;
using EventStore.Client;
using Infrastructure.Projections.InternalProjections.Repository;
using Microsoft.Extensions.Logging;

namespace Application.Group
{
    public class GroupIndexRepository : IndexProjectionRepository
    {
        protected override string StreamName { get; init; } =
            $"{InternalProjectionName.GroupCodeIndex}-res";
        protected override string EmailIndexedEvent { get; init; } = "GroupCodeIndexed";

        public GroupIndexRepository(
            EventStoreClient eventStoreClient,
            ILogger<IndexProjectionRepository> logger
        )
            : base(eventStoreClient, logger) { }
    }
}
