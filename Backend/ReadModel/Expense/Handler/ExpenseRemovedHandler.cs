using Core.Common.DataStructures;
using Core.Common.Projections;
using Core.Expense.Events;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.Expense.Handler
{
    public sealed class ExpenseRemovedHandler : IEventNotificationHandler<ExpenseRemoved>
    {
        private readonly ApplicationContext _context;

        public ExpenseRemovedHandler(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Handle(EventNotification<ExpenseRemoved> notification, CancellationToken cancellationToken)
        {
            var @event = notification.Event;

            var expense = await _context.Set<ExpenseEntity>().Where(e => e.Id == @event.Id).FirstOrDefaultAsync();

            if (expense is null)
            {
                return;
            }

            var balances = await _context
                .Set<BalanceEntity>()
                .Where(e => e.GroupId == expense.GroupId)
                .ToListAsync();

            var graph = BuildBalancesGraph(expense, balances);
            UpdateBalances(balances, graph);

            _context.Set<ExpenseEntity>().Remove(expense);
            await _context.SaveChangesAsync();
        }

        private static Graph<decimal> BuildBalancesGraph(ExpenseEntity expense, List<BalanceEntity> balances)
        {
            Graph<decimal> graph = new();

            foreach (var balance in balances)
            {
                graph.SetValue(balance.DeptorId, balance.PayerId, balance.Balance);
            }

            foreach (var deptor in expense.Deptors)
            {
                var balance = graph.GetValue(deptor.UserId, expense.PayerId) ?? 0;

                var newBalance = balance - deptor.Amount;

                if (newBalance >= 0)
                {
                    graph.SetValue(deptor.UserId, expense.PayerId, newBalance);
                }
                else
                {
                    graph.SetValue(deptor.UserId, expense.PayerId, 0);
                    graph.SetValue(expense.PayerId, deptor.UserId, decimal.Abs(newBalance));
                }
            }

            graph.MinimizeEdges();

            return graph;
        }

        private static void UpdateBalances(List<BalanceEntity> balances, Graph<decimal> graph)
        {
            var reducedBalances = graph.GetEdges();

            foreach (var balance in balances)
            {
                var reduced = reducedBalances.FirstOrDefault(e =>
                    e.Row == balance.DeptorId && e.Column == balance.PayerId
                );

                if (reduced == default)
                {
                    balance.Balance = 0;
                }
                else
                {
                    balance.Balance = reduced.Weight;
                }
            }
        }
    }
}
