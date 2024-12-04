using Core.ProjectionEntities;

namespace API.Group.DTO
{
    public sealed class BalanceDto
    {
        public Guid Id { get; set; }
        public Guid PayerId { get; set; }
        public Guid DeptorId { get; set; }
        public decimal Balance { get; set; }

        public static BalanceDto FromEntity(BalanceEntity entity)
        {
            BalanceDto dto =
                new()
                {
                    Id = entity.Id,
                    PayerId = entity.PayerId,
                    DeptorId = entity.DeptorId,
                    Balance = entity.Balance,
                };

            return dto;
        }
    }
}
