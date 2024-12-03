using System.ComponentModel.DataAnnotations;
using Core.Group;

namespace API.Expense.DTO
{
    public sealed record AddExpenseWithPaymentDto
    {
        [Required, MinLength(1), MaxLength(50)]
        public string Name { get; init; }

        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal Amount { get; init; }

        [Required]
        public Currency Currency { get; init; }

        [Required]
        public Guid GroupId { get; init; }

        [Required]
        public Guid ReceiverId { get; init; }
    }
}
