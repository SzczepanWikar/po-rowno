using System.ComponentModel.DataAnnotations;
using Core.Group;

namespace API.Group.DTO
{
    public record CreateGroupDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; init; }

        [Required]
        [StringLength(400)]
        public string Description { get; init; }

        [Required]
        public Currency Currency { get; init; }
    }
}
