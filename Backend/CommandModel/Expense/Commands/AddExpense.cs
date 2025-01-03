﻿using CommandModel.Group;
using Core.Common.Exceptions;
using Core.Expense;
using Core.Expense.Events;
using Core.Group;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace CommandModel.Expense.Commands
{
    using Expense = Core.Expense.Expense;
    using User = Core.User.User;

    public sealed record AddExpense(
        string Name,
        decimal Amount,
        Currency Currency,
        ExpenseType Type,
        Guid GroupId,
        IReadOnlyList<Deptor> Deptors,
        User User
    ) : IRequest<Guid>;

    public sealed class AddExpenseHandler : IRequestHandler<AddExpense, Guid>
    {
        private readonly IGroupService _groupService;
        private readonly IEventStoreRepository<Expense> _expenseRepository;

        public AddExpenseHandler(
            IGroupService groupService,
            IEventStoreRepository<Expense> expenseRepository
        )
        {
            _groupService = groupService;
            _expenseRepository = expenseRepository;
        }

        public async Task<Guid> Handle(AddExpense request, CancellationToken cancellationToken)
        {
            var group = await _groupService.FindOneAsync(request.GroupId, cancellationToken);

            Validate(request, group);
            var res = await AppendEvents(request, cancellationToken);

            return res;
        }

        private async Task<Guid> AppendEvents(
            AddExpense request,
            CancellationToken cancellationToken
        )
        {
            var expenseCreatedEvent = new ExpenseCreated(
                Guid.NewGuid(),
                request.Name,
                request.Amount,
                request.Currency,
                request.Type,
                request.GroupId,
                request.User.Id,
                request.Deptors
            );

            await _expenseRepository.CreateAsync(
                expenseCreatedEvent.Id,
                expenseCreatedEvent,
                cancellationToken
            );

            await _groupService.AppendAsync(
                request.GroupId,
                new ExpenseAddedToGroup(request.GroupId, expenseCreatedEvent.Id)
            );

            return expenseCreatedEvent.Id;
        }

        private static void Validate(AddExpense request, Core.Group.Group group)
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
                throw new BadRequestException("Amunt must be greather than 0.");
            }

            if (request.Currency != group.Currency)
            {
                throw new BadRequestException("Currencies not match.");
            }

            ValidateDeptors(request, group);
        }

        private static void ValidateDeptors(AddExpense request, Core.Group.Group group)
        {
            var deptors = request.Deptors;
            var dept = deptors.Aggregate<Deptor, decimal, decimal>(
                0,
                (acc, curr) => acc + curr.Amount,
                acc => acc
            );

            if (dept > request.Amount)
            {
                throw new BadRequestException("Deptors cannot pay more than amount.");
            }

            if (!deptors.Select(e => e.UserId).ToHashSet().IsSubsetOf(group.UsersIds.ToHashSet()))
            {
                throw new BadRequestException("All deptors must be part of group.");
            }

            if (deptors.Any(e => request.User.Id == e.UserId))
            {
                throw new BadRequestException("Payer cannot be a deptor.");
            }

            if (request.Type == ExpenseType.Settlement && deptors.Count != 1)
            {
                throw new BadRequestException("Settlement require only one deptor.");
            }
        }
    }
}
