using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.Expense.Handler
{
    public sealed record GetOneExpense(Guid Id, Core.User.User User) : IRequest<ExpenseEntity>;

    public sealed class GetOneExpenseHandler : IRequestHandler<GetOneExpense, ExpenseEntity>
    {
        private readonly ApplicationContext _context;

        public GetOneExpenseHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ExpenseEntity> Handle(
            GetOneExpense request,
            CancellationToken cancellationToken
        )
        {
            var query = _context
                .Set<ExpenseEntity>()
                .Include(e => e.Deptors)
                .Include(e => e.Payer)
                .Where(e => e.Id == request.Id);

            return await query.FirstOrDefaultAsync();
        }
    }
}
