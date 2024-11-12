using Core.Common.DataStructures;
using Microsoft.EntityFrameworkCore;

namespace ReadModel.Expense
{
    public sealed class BalanceCalculator
    {
        private readonly ApplicationContext _context;

        public BalanceCalculator(ApplicationContext context)
        {
            _context = context;
        }

        public async Task CalcBalancesAsync(
            ExpenseEntity expense,
            CancellationToken cancellationToken
        )
        {
            var currentBalances = await _context
                .Set<BalanceEntity>()
                .Where(e => e.GroupId == expense.GroupId)
                .ToListAsync(cancellationToken);

            if (currentBalances.Count == 0)
            {
                var newBalances = expense.Deptors.Select(e => new BalanceEntity
                {
                    Id = Guid.NewGuid(),
                    PayerId = expense.PayerId,
                    DeptorId = e.UserId,
                    GroupId = expense.GroupId,
                    Balance = e.Amount,
                });

                currentBalances.AddRange(newBalances);
                await _context.AddRangeAsync(currentBalances);
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            var balancesGraph = GenerateBalancesGraph(expense, currentBalances);

            balancesGraph.MinimizeEdges();

            var balances = UpdateBalances(expense, currentBalances, balancesGraph);

            foreach (var balance in balances)
            {
                if (_context.Entry(balance).State == EntityState.Detached)
                {
                    await _context.AddAsync(balance);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static List<BalanceEntity> UpdateBalances(
            ExpenseEntity @event,
            List<BalanceEntity> currentBalances,
            Graph<decimal> balancesGraph
        )
        {
            var reducedBalances = balancesGraph.GetEdges();

            foreach (var balance in currentBalances)
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

            foreach (var balance in reducedBalances)
            {
                var current = currentBalances.FirstOrDefault(e =>
                    e.DeptorId == balance.Row && e.PayerId == balance.Column
                );

                if (current is null)
                {
                    BalanceEntity newBalance =
                        new()
                        {
                            Id = Guid.NewGuid(),
                            PayerId = balance.Column,
                            DeptorId = balance.Row,
                            GroupId = @event.GroupId,
                            Balance = balance.Weight,
                        };

                    currentBalances.Add(newBalance);
                }
            }

            return currentBalances;
        }

        private static Graph<decimal> GenerateBalancesGraph(
            ExpenseEntity @event,
            List<BalanceEntity> currentBalances
        )
        {
            Graph<decimal> balancesGraph = new();

            foreach (var balance in currentBalances)
            {
                balancesGraph.SetValue(balance.DeptorId, balance.PayerId, balance.Balance);
            }

            foreach (var deptor in @event.Deptors)
            {
                var balance = balancesGraph.GetValue(deptor.UserId, @event.PayerId) ?? 0;

                balance += deptor.Amount;

                balancesGraph.SetValue(deptor.UserId, @event.PayerId, balance);
            }

            return balancesGraph;
        }
    }
}
