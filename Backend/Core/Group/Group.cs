using Core.Common.Aggregate;
using Core.Common.Code;
using Core.Group.Events;

namespace Core.Group
{
    public enum Currency
    {
        Dollar,
        Euro,
        PolishZloty
    }

    public enum GroupCodeType
    {
        Join
    }

    public sealed class Group : Aggregate
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Currency Currency { get; private set; }
        public Guid OwnerId { get; private set; }
        public IList<Guid> UsersIds { get; init; } = new List<Guid>();
        public IList<Guid> BannedUsersIds { get; init; } = new List<Guid>();
        public Codes<GroupCodeType> Codes { get; init; } = new();
        public IList<Guid> ExpensesIds { get; init; } = new List<Guid>();

        public override void When(object @event)
        {
            switch (@event)
            {
                case GroupCreated(
                    Guid id,
                    string name,
                    string description,
                    Currency currency,
                    Guid ownerId
                ):
                    Id = id;
                    Name = name;
                    Description = description;
                    Currency = currency;
                    OwnerId = ownerId;
                    UsersIds.Add(ownerId);
                    break;
                case GroupCodeGenerated(_, Code<GroupCodeType> code):
                    Codes.Push(code);
                    break;
                case UserJoinedGroup(_, Guid userId):
                    UsersIds.Add(userId);
                    break;
                case GroupDataUpdated(_, var name, var description, var ownerId):
                    Name = name ?? Name;
                    Description = description ?? Description;
                    OwnerId = ownerId ?? OwnerId;
                    break;
                case ExpenseAddedToGroup(_, Guid expenseId):
                    ExpensesIds.Add(expenseId);
                    break;
                case ExpenseRemovedFromGroup(_, Guid expenseId):
                    ExpensesIds.Remove(expenseId);
                    break;
                case UserBannedFromGroup(_, Guid userId):
                    UsersIds.Remove(userId);
                    BannedUsersIds.Add(userId);
                    break;
                case UserUnbannedFromGroup(_, Guid userId):
                    BannedUsersIds.Remove(userId);
                    UsersIds.Add(userId);
                    break;
                default:
                    break;
            }
        }
    }
}
