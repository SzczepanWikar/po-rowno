using Core.Common.Projections;
using EventStore.Client;
using Infrastructure.Projections.InternalProjections.Repository;
using Microsoft.Extensions.Logging;

namespace CommandModel.Expense
{
    public sealed class PaymentIndexRepository : IndexProjectionRepository
    {
        protected override string StreamName { get; init; } =
            $"{InternalProjectionName.PayPalOrderNumberIndex}-res";
        protected override string EmailIndexedEvent { get; init; } = "PayPalOrderNumberIndexed";

        public PaymentIndexRepository(
            EventStoreClient eventStoreClient,
            ILogger<IndexProjectionRepository> logger
        )
            : base(eventStoreClient, logger) { }
    }
}
