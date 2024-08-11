using System.ComponentModel.DataAnnotations;

namespace API.User.ViewModels
{
    public record SignUpDto
    {
        [Required]
        public required string Username { get; init; }

        [Required]
        [EmailAddress]
        public string Email
        {
            get => _email;
            init => _email = value.ToLower();
        }

        [Required]
        [Compare(nameof(ReapetedPassword), ErrorMessage = "Passwords not match.")]
        [Password]
        public string Password { get; init; }

        [Required]
        [Password]
        public string ReapetedPassword { get; init; }

        private string _email;
    }
}
