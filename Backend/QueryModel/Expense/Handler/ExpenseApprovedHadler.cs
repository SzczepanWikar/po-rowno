using Core.Common.Projections;
using Core.Expense.Events;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.Expense.Handler
{
    public sealed class ExpenseApprovedHadler : IEventNotificationHandler<ExpenseApproved>
    {
        private readonly ApplicationContext _context;

        public ExpenseApprovedHadler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(
            EventNotification<ExpenseApproved> notification,
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

            expense.PaymentStatus = "APPROVED";

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
