using System.ComponentModel.DataAnnotations;

namespace API.Group.DTO
{
    public record UpdateGroupDto
    {
        [StringLength(50)]
        public string? Name { get; init; }

        [StringLength(400)]
        public string? Description { get; init; }

        public Guid? OwnerId { get; set; }
    }
}
