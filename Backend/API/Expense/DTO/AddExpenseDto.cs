﻿using System.ComponentModel.DataAnnotations;
using Core.Expense;
using Core.Group;

namespace API.Expense.DTO
{
    public sealed record AddExpenseDto
    {
        [Required, MinLength(1), MaxLength(50)]
        public string Name { get; init; }

        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal Amount { get; init; }

        [Required]
        public Currency Currency { get; init; }

        [Required]
        public ExpenseType Type { get; init; }

        [Required]
        public Guid GroupId { get; init; }

        [Required, MinLength(1)]
        public IReadOnlyList<Deptor> Deptors { get; init; }
    }
}
