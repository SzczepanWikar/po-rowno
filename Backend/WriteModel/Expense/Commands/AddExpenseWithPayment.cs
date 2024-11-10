using Core.Common.Exceptions;
using Core.Common.PayPal;
using Core.Expense;
using Core.Expense.Events;
using Core.Group;
using Infrastructure.EventStore.Repository;
using Infrastructure.PayPal;
using MediatR;
using WriteModel.Group;
using WriteModel.User.Services;

namespace WriteModel.Expense.Commands
{
    using Expense = Core.Expense.Expense;
    using Group = Core.Group.Group;
    using User = Core.User.User;

    public sealed record AddExpenseWithPayment(
        decimal Amount,
        Currency Currency,
        Guid GroupId,
        Guid ReceiverId,
        User User
    ) : IRequest<ExpenseWithPaymentCreatedResult>;

    public sealed class AddExpenseWithPaymentHandler
        : IRequestHandler<AddExpenseWithPayment, ExpenseWithPaymentCreatedResult>
    {
        private readonly IGroupService _groupService;
        private readonly IEventStoreRepository<Expense> _expenseRepository;
        private readonly IPayPalService _payPalService;
        private readonly IUserService _userService;

        public AddExpenseWithPaymentHandler(
            IGroupService groupService,
            IEventStoreRepository<Expense> expenseRepository,
            IPayPalService payPalService,
            IUserService userService
        )
        {
            _groupService = groupService;
            _expenseRepository = expenseRepository;
            _payPalService = payPalService;
            _userService = userService;
        }

        public async Task<ExpenseWithPaymentCreatedResult> Handle(
            AddExpenseWithPayment request,
            CancellationToken cancellationToken
        )
        {
            var group = await _groupService.FindOneAsync(request.GroupId, cancellationToken);

            Validate(request, group);

            try
            {
                var receiver = await _userService.FindOneAsync(
                    request.ReceiverId,
                    cancellationToken
                );
                NewOrder newOrder =
                    new(request.Amount, request.Currency, receiver.Email, "PoRowno");
                var paymentResult = await _payPalService.Create(newOrder);

                var @event = new ExpenseCreated(
                    Guid.NewGuid(),
                    "",
                    request.Amount,
                    request.Currency,
                    ExpenseType.Settlement,
                    request.GroupId,
                    request.User.Id,
                    [new Deptor(request.ReceiverId, request.Amount)],
                    paymentResult
                );

                await _expenseRepository.CreateAsync(@event.Id, @event);

                return new ExpenseWithPaymentCreatedResult(@event.Id, paymentResult.Response.id);
            }
            catch (HttpRequestException ex)
            {
                throw new BadGatewayException("Payment system error.");
            }
        }

        private static void Validate(AddExpenseWithPayment request, Group group)
        {
            if (!group.UsersIds.Any(e => request.User.Id == e))
            {
                throw new ForbiddenException();
            }

            if (group.BannedUsersIds.Any(e => request.User.Id == e))
            {
                throw new ForbiddenException();
            }

            if (request.Amount <= 0)
            {
                throw new BadRequestException("Amunt must be greather than 0");
            }

            if (request.Currency != group.Currency)
            {
                throw new BadRequestException("Currencies not match");
            }

            if (!group.UsersIds.Any(e => e == request.ReceiverId))
            {
                throw new BadRequestException("All deptors must be part of group");
            }

            if (request.ReceiverId == request.User.Id)
            {
                throw new BadRequestException("Payer cannot be receiver");
            }
        }
    }

    public class PaymentResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<PaymentResponseLink> links { get; set; }
    }

    public class PaymentResponseLink
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }
}
