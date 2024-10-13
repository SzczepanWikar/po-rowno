using Core.Common.Projections;
using Core.Expense.Events;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.Expense.Handler
{
    public sealed class ExpensePaymentCapturedHandler
        : IEventNotificationHandler<ExpensePaymentCaptured>
    {
        private readonly ApplicationContext _context;

        public ExpensePaymentCapturedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<ExpensePaymentCaptured> notification,
            CancellationToken cancellationToken
        )
        {
            var expense = await _context
                .Set<ExpenseEntity>()
                .Where(e => e.Id == notification.Event.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (expense is null)
            {
                return;
            }

            expense.PaymentStatus = notification.Event.CapturedOrder.Response.status;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
