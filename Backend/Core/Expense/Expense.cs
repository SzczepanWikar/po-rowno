﻿using Core.Common.Aggregate;
using Core.Common.PayPal;
using Core.Expense.Events;
using Core.Group;

namespace Core.Expense
{
    public enum ExpenseType
    {
        Cost = 0,
        Settlement,
    }

    public sealed class Expense : Aggregate
    {
        public string Name { get; private set; } = String.Empty;
        public decimal Amount { get; private set; }
        public Currency Currency { get; private set; }
        public ExpenseType Type { get; private set; }
        public Guid GroupId { get; private set; }
        public Guid PayerId { get; private set; }
        public IReadOnlyList<Deptor> Deptors { get; private set; } = new List<Deptor>();
        public Payment Payment { get; private set; } = new();

        public override void When(object @event)
        {
            switch (@event)
            {
                case ExpenseCreated e:
                    OnExpenseCreated(e);
                    break;
                case ExpensePaymentCaptured(_, CapturedOrder capturedOrder):
                    Payment.Status = capturedOrder.Response.status;
                    break;
                case ExpenseRemoved:
                    Deleted = true;
                    break;
            }
        }

        private void OnExpenseCreated(ExpenseCreated e)
        {
            Id = e.Id;
            Name = e.Name;
            Amount = e.Amount;
            Currency = e.Currency;
            Type = e.Type;
            GroupId = e.GroupId;
            PayerId = e.PayerId;
            Deptors = e.Deptors;

            if (e.Payment is not null)
            {
                Payment.Status = e.Payment.Response.status;
                Payment.Id = e.Payment.Response.id;
            }
        }
    }
}
