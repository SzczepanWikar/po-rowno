using API.User.DTO;
using Core.Group;
using QueryModel.Expense;
using QueryModel.Group;
using QueryModel.User;
using QueryModel.UserGroup;

namespace API.Group.DTO
{
    public sealed class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? JoinCode { get; set; }
        public DateTime? JoinCodeValidTo { get; set; }
        public Currency Currency { get; set; }
        public Guid OwnerId { get; set; }
        public UserDto? Owner { get; set; }
        public ICollection<UserGroupDto> UserGroups { get; set; } = new List<UserGroupDto>();
        public ICollection<BalanceDto> Balances { get; set; } = new List<BalanceDto>();

        public static GroupDto FromEntity(GroupEntity entity)
        {
            GroupDto dto =
                new()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    JoinCode = entity.JoinCode,
                    JoinCodeValidTo = entity.JoinCodeValidTo,
                    Currency = entity.Currency,
                    OwnerId = entity.OwnerId,
                };

            if (entity.Owner is not null)
            {
                dto.Owner = UserDto.FromGroupRelatedEntity(entity.Owner);
            }

            if (entity.UserGroups?.Count > 0)
            {
                dto.UserGroups = entity
                    .UserGroups.Select(UserGroupDto.FromGroupRelatedEntity)
                    .ToArray();
            }

            if (entity.Balances?.Count > 0)
            {
                dto.Balances = entity.Balances.Select(BalanceDto.FromEntity).ToArray();
            }

            return dto;
        }

        public static GroupDto FromUserGroupRelatedEntity(GroupEntity entity)
        {
            GroupDto dto =
                new()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    JoinCode = entity.JoinCode,
                    JoinCodeValidTo = entity.JoinCodeValidTo,
                    Currency = entity.Currency,
                    OwnerId = entity.OwnerId,
                };

            if (entity.Owner is not null)
            {
                dto.Owner = UserDto.FromGroupRelatedEntity(entity.Owner);
            }

            if (entity.Balances?.Count > 0)
            {
                dto.Balances = entity.Balances.Select(BalanceDto.FromEntity).ToArray();
            }

            return dto;
        }
    }
}
