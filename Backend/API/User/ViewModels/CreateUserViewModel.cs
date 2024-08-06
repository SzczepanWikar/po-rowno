using System.ComponentModel.DataAnnotations;

namespace API.User.ViewModels
{
    public record CreateUserViewModel
    {
        [Required]
        public string Username { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        [Compare(nameof(ReapetedPassword), ErrorMessage = "Passwords not match.")]
        public string Password { get; init; }

        [Required]
        public string ReapetedPassword { get; init; }
    }
}
