using CommandModel.Group;
using Core.Common.Exceptions;
using Core.Common.PayPal;
using Core.Common.Projections;
using Core.Expense.Events;
using Infrastructure.EventStore.Repository;
using Infrastructure.PayPal;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CommandModel.Expense.Commands
{
    using Expense = Core.Expense.Expense;
    using Group = Core.Group.Group;
    using User = Core.User.User;

    public sealed record CapturePayment(string OrderNumber, User User)
        : IRequest<OrderCapturedResponse>;

    public sealed class CapturePaymentHandler
        : IRequestHandler<CapturePayment, OrderCapturedResponse>
    {
        private readonly IGroupService _groupService;
        private readonly IEventStoreRepository<Expense> _expenseRepository;
        private readonly IPayPalService _payPalService;
        private readonly IIndexProjectionRepository _indexProjectionRepository;

        public CapturePaymentHandler(
            IGroupService groupService,
            IEventStoreRepository<Expense> expenseRepository,
            IPayPalService payPalService,
            [FromKeyedServices(InternalProjectionName.PayPalOrderNumberIndex)]
                IIndexProjectionRepository indexProjectionRepository
        )
        {
            _groupService = groupService;
            _expenseRepository = expenseRepository;
            _payPalService = payPalService;
            _indexProjectionRepository = indexProjectionRepository;
        }

        public async Task<OrderCapturedResponse> Handle(
            CapturePayment request,
            CancellationToken cancellationToken
        )
        {
            var expenseId = await _indexProjectionRepository.GetOwnerId(request.OrderNumber);

            if (!expenseId.HasValue)
            {
                throw new NotFoundException("Payment not found.");
            }

            var expense = await _expenseRepository.FindOneAsync(expenseId.Value, cancellationToken);

            if (expense is null)
            {
                throw new NotFoundException("Expense not found.");
            }

            if (request.User.Id != expense.PayerId)
            {
                throw new BadRequestException("User is not payer in this expense.");
            }

            try
            {
                var paymentResult = await _payPalService.Capture(request.OrderNumber);

                var @event = new ExpensePaymentCaptured(expenseId.Value, paymentResult);
                await _expenseRepository.AppendAsync(expenseId.Value, @event);

                return paymentResult.Response;
            }
            catch (HttpRequestException ex)
            {
                var @event = new PaymentHttpErrorOccured(expenseId.Value, ex.Message);
                await _expenseRepository.AppendAsync(expenseId.Value, @event);

                throw new BadGatewayException("Payment system error.");
            }
        }
    }
}
