using System.ComponentModel.DataAnnotations;

namespace API.User.DTO
{
    public sealed record RequestPasswordResetDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; init; } = string.Empty;
    }
}
