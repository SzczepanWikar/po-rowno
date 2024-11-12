using Core.Expense.Events;
using Infrastructure.EventStore.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using WriteModel.Group;

namespace WriteModel.Expense.Commands
{
    using Expense = Core.Expense.Expense;

    public sealed record ApproveOrCancelExpense(Guid Id, bool Success) : IRequest;

    public sealed class ApproveOrCancelExpenseHandler : IRequestHandler<ApproveOrCancelExpense>
    {
        private readonly IEventStoreRepository<Expense> _expenseRepository;
        private readonly ILogger<ApproveOrCancelExpenseHandler> _logger;

        public ApproveOrCancelExpenseHandler(
            IEventStoreRepository<Expense> expenseRepository,
            ILogger<ApproveOrCancelExpenseHandler> logger
        )
        {
            _expenseRepository = expenseRepository;
            _logger = logger;
        }

        public async Task Handle(
            ApproveOrCancelExpense request,
            CancellationToken cancellationToken
        )
        {
            var id = request.Id;
            var expense = await _expenseRepository.FindOneAsync(id, cancellationToken);

            if (expense is null)
            {
                _logger.LogWarning($"Expense of id {id} does not exist.");
                return;
            }

            if (expense.Payment.Status == "COMPLETED")
            {
                _logger.LogWarning($"Expense of id {id} is completed.");
                return;
            }

            if (request.Success)
            {
                ExpenseApproved @event = new(id);
                await _expenseRepository.AppendAsync(id, @event, cancellationToken);
            }
            else
            {
                ExpenseCanceled @event = new(id);
                await _expenseRepository.AppendAsync(id, @event, cancellationToken);
            }
        }
    }
}
