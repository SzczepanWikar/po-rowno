using API.User.DTO;
using ReadModel.Expense;

namespace API.Expense.DTO
{
    public sealed class DeptorDto
    {
        public Guid Id { get; set; }
        public Guid ExpenseId { get; set; }
        public decimal Amount { get; set; }
        public Guid UserId { get; set; }
        public ExpenseDto Expense { get; set; }
        public UserDto User { get; set; }

        public static DeptorDto FromEntity(ExpenseDeptorEntity entity)
        {
            DeptorDto dto =
                new()
                {
                    Id = entity.Id,
                    Amount = entity.Amount,
                    UserId = entity.UserId,
                    ExpenseId = entity.ExpenseId,
                };

            if (entity.Expense is not null)
            {
                dto.Expense = ExpenseDto.FromEntity(entity.Expense);
            }

            if (entity.User is not null)
            {
                dto.User = UserDto.FromEntity(entity.User);
            }

            return dto;
        }

        public static DeptorDto FromDeptorEntity(ExpenseDeptorEntity entity)
        {
            DeptorDto dto =
                new()
                {
                    Id = entity.Id,
                    Amount = entity.Amount,
                    UserId = entity.UserId,
                    ExpenseId = entity.ExpenseId,
                };

            if (entity.User is not null)
            {
                dto.User = UserDto.FromEntity(entity.User);
            }

            return dto;
        }
    }
}
