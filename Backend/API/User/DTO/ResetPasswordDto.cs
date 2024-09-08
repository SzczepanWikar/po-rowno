using System.ComponentModel.DataAnnotations;
using Infrastructure.Attributes;

namespace API.User.DTO
{
    public sealed record ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        [Password]
        public string Password { get; init; }

        [Required]
        [Password]
        [Compare(nameof(ConfirmPassword), ErrorMessage = "Passwords not match.")]
        public string ConfirmPassword { get; init; }

        [Required]
        public string Code { get; init; }
    }
}
