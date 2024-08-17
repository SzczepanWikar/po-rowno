using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Attributes
{
    public class InFutureAttribute : ValidationAttribute
    {
        public InFutureAttribute()
        {
            ErrorMessage = "The date must be in the future.";
        }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext
        )
        {
            if (value is DateTime dateTimeValue)
            {
                if (dateTimeValue > DateTime.Now)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return new ValidationResult("Invalid date format.");
        }
    }
}
