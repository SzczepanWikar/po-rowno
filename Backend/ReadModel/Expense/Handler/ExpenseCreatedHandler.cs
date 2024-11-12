using Core.Common.DataStructures;
using Core.Common.Projections;
using Core.Expense.Events;
using Microsoft.EntityFrameworkCore;
using ReadModel.User;
using static Grpc.Core.Metadata;

namespace ReadModel.Expense.Handler
{
    public sealed class ExpenseCreatedHandler : IEventNotificationHandler<ExpenseCreated>
    {
        private readonly ApplicationContext _context;
        private readonly BalanceCalculator _balanceCalculator;

        public ExpenseCreatedHandler(
            ApplicationContext context,
            BalanceCalculator balanceCalculator
        )
        {
            _context = context;
            _balanceCalculator = balanceCalculator;
        }

        public async Task Handle(
            EventNotification<ExpenseCreated> notification,
            CancellationToken cancellationToken
        )
        {
            var @event = notification.Event;
            var expense = new ExpenseEntity
            {
                Id = @event.Id,
                Name = @event.Name,
                Amount = @event.Amount,
                Currency = @event.Currency,
                Type = @event.Type,
                PayerId = @event.PayerId,
                PaymentId = @event.Payment?.Response.id,
                PaymentStatus = @event.Payment?.Response.status,
                GroupId = @event.GroupId,
            };

            var payer = await _context
                .Set<UserEntity>()
                .Where(e => e.Id == @event.PayerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (payer is null)
            {
                return;
            }

            expense.Payer = payer;

            var deptors = @event
                .Deptors.Select(e => new ExpenseDeptorEntity
                {
                    Id = Guid.NewGuid(),
                    ExpenseId = @event.Id,
                    UserId = e.UserId,
                    Amount = e.Amount,
                })
                .ToList();

            expense.Deptors = deptors;

            await _context.AddAsync(expense, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            if (expense.PaymentStatus is null || expense.PaymentStatus == "COMPLETED")
            {
                await _balanceCalculator.CalcBalancesAsync(expense, cancellationToken);
            }
        }
    }
}
