using System.ComponentModel.DataAnnotations;

namespace API.User.ViewModels
{
    public record SignInDto
    {
        [Required]
        public string Email { get; init; } = String.Empty;

        [Required]
        public string Password { get; init; } = String.Empty;
    }
}
