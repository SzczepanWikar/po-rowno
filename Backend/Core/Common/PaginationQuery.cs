namespace Core.Common
{
    public record PaginationQuery(int Page = 1, int Take = 10, bool Ascending = true);
}
