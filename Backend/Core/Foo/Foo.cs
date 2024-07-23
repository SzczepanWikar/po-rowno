using Core.Common.Aggregate;

/// <summary>
/// Namespace with classes and records created for Infrastructure testing.
/// </summary>
namespace Core.Foo
{
    /// <summary>
    /// Aggragate for infrastructure testing.
    /// </summary>
    public sealed class Foo : Aggregate
    {
        public string Name { get; set; }
        public int SomeNumber { get; set; }

        public override void When(object @event)
        {
            switch (@event)
            {
                case FooCreated(Guid id, string name, int someNumber):
                    Id = id;
                    Name = name;
                    SomeNumber = someNumber;
                    Deleted = false;
                    break;
                case FooUpdated(_, int someNumber):
                    SomeNumber = someNumber;
                    break;
                case FooDeleted(_):
                    Deleted = true;
                    break;
                default:
                    return;
            }
        }
    }

    public record FooCreated(Guid Id, string Name, int SomeNumber);

    public record FooUpdated(Guid Id, int SomeNumber);

    public record FooDeleted(Guid Id);
}
