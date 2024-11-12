using Core.Common.DataStructures;
using Core.Common.Projections;
using Core.Expense.Events;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.Expense.Handler
{
    public sealed class ExpensePaymentCapturedHandler
        : IEventNotificationHandler<ExpensePaymentCaptured>
    {
        private readonly ApplicationContext _context;
        private readonly BalanceCalculator _balanceCalculator;

        public ExpensePaymentCapturedHandler(
            ApplicationContext context,
            BalanceCalculator balanceCalculator
        )
        {
            _context = context;
            _balanceCalculator = balanceCalculator;
        }

        public async Task Handle(
            EventNotification<ExpensePaymentCaptured> notification,
            CancellationToken cancellationToken
        )
        {
            var expense = await _context
                .Set<ExpenseEntity>()
                .Include(e => e.Deptors)
                .Where(e => e.Id == notification.Event.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (expense is null)
            {
                return;
            }

            expense.PaymentStatus = notification.Event.CapturedOrder.Response.status;

            await _context.SaveChangesAsync(cancellationToken);

            await _balanceCalculator.CalcBalancesAsync(expense, cancellationToken);
        }
    }
}
