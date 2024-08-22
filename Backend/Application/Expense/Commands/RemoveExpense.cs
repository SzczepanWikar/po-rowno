using Application.Group;
using Core.Common.Exceptions;
using Core.Expense.Events;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace Application.Expense.Commands
{
    using Expense = Core.Expense.Expense;
    using User = Core.User.User;

    public sealed record RemoveExpense(Guid Id, User User) : IRequest;

    public sealed class RemoveExpenseHandler : IRequestHandler<RemoveExpense>
    {
        private readonly IGroupService _groupService;
        private readonly IEventStoreRepository<Expense> _expenseRepository;

        public RemoveExpenseHandler(
            IGroupService groupService,
            IEventStoreRepository<Expense> expenseRepository
        )
        {
            _groupService = groupService;
            _expenseRepository = expenseRepository;
        }

        public async Task Handle(RemoveExpense request, CancellationToken cancellationToken)
        {
            var expense = await _expenseRepository.Find(request.Id, cancellationToken);

            if (expense == null || expense.Deleted)
            {
                return;
            }

            if (expense.PayerId != request.User.Id && expense.GroupId != request.User.Id)
            {
                throw new ForbiddenException();
            }

            await RemoveExpense(request, cancellationToken);
            await RemoveFromGroup(request, expense, cancellationToken);
        }

        private async Task RemoveExpense(RemoveExpense request, CancellationToken cancellationToken)
        {
            var @event = new ExpenseRemoved(request.Id);
            await _expenseRepository.Append(request.Id, @event, cancellationToken);
        }

        private async Task RemoveFromGroup(
            RemoveExpense request,
            Expense expense,
            CancellationToken cancellationToken
        )
        {
            var @event = new ExpenseRemovedFromGroup(expense.GroupId, request.Id);
            await _groupService.Append(expense.GroupId, @event, cancellationToken);
        }
    }
}
