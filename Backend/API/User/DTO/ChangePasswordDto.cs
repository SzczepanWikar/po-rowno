using System.ComponentModel.DataAnnotations;
using Infrastructure.Attributes;

namespace API.User.DTO
{
    public sealed record ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; init; } = string.Empty;

        [Password]
        [Required]
        public string NewPassword { get; init; } = string.Empty;
    }
}
