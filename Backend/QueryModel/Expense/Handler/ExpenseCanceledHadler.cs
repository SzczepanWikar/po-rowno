using Core.Common.Projections;
using Microsoft.EntityFrameworkCore;

namespace QueryModel.Expense.Handler
{
    public sealed class ExpenseCanceledHadler : IEventNotificationHandler<ExpenseCanceled>
    {
        private readonly ApplicationContext _context;

        public ExpenseCanceledHadler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<ExpenseCanceled> notification,
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

            expense.PaymentStatus = "CANCELED";

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
