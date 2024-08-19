namespace Infrastructure.Projections.InternalProjections.Repository
{
    public struct IndexedValueEvent
    {
        public string IndexedValue { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
