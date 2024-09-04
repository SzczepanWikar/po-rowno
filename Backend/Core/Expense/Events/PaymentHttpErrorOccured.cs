namespace Core.Expense.Events
{
    public sealed record PaymentHttpErrorOccured(Guid Id, string Payload);
}
