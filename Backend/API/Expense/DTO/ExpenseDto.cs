using API.Group.DTO;
using API.User.DTO;
using Core.Expense;
using Core.Group;
using ReadModel.Expense;
using ReadModel.Group;
using ReadModel.User;

namespace API.Expense.DTO
{
    public sealed class ExpenseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public ExpenseType Type { get; set; }
        public Guid PayerId { get; set; }
        public Guid GroupId { get; set; }
        public string? PaymentStatus { get; set; }
        public GroupDto Group { get; set; }
        public UserDto Payer { get; set; }
        public ICollection<DeptorDto> Deptors { get; set; } = new List<DeptorDto>();

        public static ExpenseDto FromEntity(ExpenseEntity entity)
        {
            ExpenseDto dto =
                new()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Amount = entity.Amount,
                    Currency = entity.Currency,
                    Type = entity.Type,
                    PayerId = entity.PayerId,
                    GroupId = entity.GroupId,
                    PaymentStatus = entity.PaymentStatus,
                };

            if (entity.Group is not null)
            {
                dto.Group = GroupDto.FromEntity(entity.Group);
            }

            if (entity.Payer is not null)
            {
                dto.Payer = UserDto.FromEntity(entity.Payer);
            }

            if (entity.Deptors.Count > 0)
            {
                dto.Deptors = entity.Deptors.Select(e => DeptorDto.FromEntity(e)).ToArray();
            }

            return dto;
        }
    }
}
