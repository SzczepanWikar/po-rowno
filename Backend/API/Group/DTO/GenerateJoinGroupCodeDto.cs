using System.ComponentModel.DataAnnotations;
using Infrastructure.Attributes;

namespace API.Group.DTO
{
    public record GenerateJoinGroupCodeDto
    {
        [Required]
        [InFuture]
        public DateTime ValidTo { get; init; }
    }
}
