using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public sealed class PasswordAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var password = value as string;
        var displayedName = validationContext.DisplayName;

        if (password == null)
        {
            return new ValidationResult($"{displayedName} cannot be null");
        }

        if (password.Length < 12)
        {
            return new ValidationResult($"{displayedName} must be at least 12 characters long.");
        }

        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasLowerChar = new Regex(@"[a-z]+");
        var hasSymbols = new Regex(@"[!@#$%^&*(),.?""{}|<>]+");

        if (!hasNumber.IsMatch(password))
        {
            return new ValidationResult(
                $"{displayedName} must contain at least one numeric character."
            );
        }
        if (!hasUpperChar.IsMatch(password))
        {
            return new ValidationResult(
                $"{displayedName} must contain at least one uppercase letter."
            );
        }
        if (!hasLowerChar.IsMatch(password))
        {
            return new ValidationResult(
                $"{displayedName} must contain at least one lowercase letter."
            );
        }
        if (!hasSymbols.IsMatch(password))
        {
            return new ValidationResult(
                $"{displayedName} must contain at least one special character."
            );
        }

        return ValidationResult.Success;
    }
}
