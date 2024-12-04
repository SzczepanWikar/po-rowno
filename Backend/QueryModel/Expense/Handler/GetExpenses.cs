using Core.ProjectionEntities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace QueryModel.Expense.Handler
{
    public sealed record GetExpenses(
        Guid GroupId,
        Core.User.User User,
        int Page,
        int Take,
        bool Ascending
    ) : IRequest<IEnumerable<ExpenseEntity>>;

    public sealed class GetExpensesHandler
        : IRequestHandler<GetExpenses, IEnumerable<ExpenseEntity>>
    {
        private readonly ApplicationContext _context;

        public GetExpensesHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExpenseEntity>> Handle(
            GetExpenses request,
            CancellationToken cancellationToken
        )
        {
            var query = _context
                .Set<ExpenseEntity>()
                .Include(e => e.Deptors)
                .Include(e => e.Payer)
                .Where(e =>
                    e.GroupId == request.GroupId
                    && e.Group.UserGroups.Any(ug => ug.UserId == request.User.Id)
                    && (e.PaymentStatus == "COMPLETED" || e.PaymentStatus == null)
                );

            if (request.Ascending)
            {
                query = query.OrderBy(e => e.CreatedAt);
            }
            else
            {
                query = query.OrderByDescending(e => e.CreatedAt);
            }

            var skip = (request.Page - 1 < 0 ? 0 : request.Page - 1) * request.Take;

            query = query.Skip(skip).Take(request.Take);

            return await query.ToListAsync();
        }
    }
}
