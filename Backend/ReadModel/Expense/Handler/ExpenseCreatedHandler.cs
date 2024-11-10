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

        public ExpenseCreatedHandler(ApplicationContext context)
        {
            _context = context;
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
                await CalcBalancesAsync(@event, cancellationToken);
            }
        }

        private async Task CalcBalancesAsync(
            ExpenseCreated @event,
            CancellationToken cancellationToken
        )
        {
            var currentBalances = await _context
                .Set<BalanceEntity>()
                .Where(e => e.GroupId == @event.GroupId)
                .ToListAsync(cancellationToken);

            if (currentBalances.Count == 0)
            {
                var newBalances = @event.Deptors.Select(e => new BalanceEntity
                {
                    Id = Guid.NewGuid(),
                    PayerId = @event.PayerId,
                    DeptorId = e.UserId,
                    GroupId = @event.GroupId,
                    Balance = e.Amount,
                });

                currentBalances.AddRange(newBalances);
                await _context.AddRangeAsync(currentBalances);
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            var balancesGraph = GenerateBalancesGraph(@event, currentBalances);

            balancesGraph.MinimizeEdges();

            var balances = await UpdateBalances(@event, currentBalances, balancesGraph);

            foreach (var balance in balances)
            {
                if (_context.Entry(balance).State == EntityState.Detached)
                {
                    await _context.AddAsync(balance);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static async Task<List<BalanceEntity>> UpdateBalances(
            ExpenseCreated @event,
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
            ExpenseCreated @event,
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
