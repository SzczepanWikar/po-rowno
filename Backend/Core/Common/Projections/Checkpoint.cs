namespace Core.Common.Projections
{
    public record Checkpoint(string SubscritpionId, ulong? Position, DateTime CreatedAt);
}
