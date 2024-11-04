using Core.Common;

namespace API.Expense.DTO
{
    public sealed record GetExpensesQueryDto(Guid GroupId) : PaginationQuery;
}
