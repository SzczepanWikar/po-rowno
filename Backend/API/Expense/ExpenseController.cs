using API.Expense.DTO;
using Application.Expense.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Expense
{
    using User = Core.User.User;

    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpenseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] AddExpenseDto dto)
        {
            var user = HttpContext.Items["User"] as User;

            var request = new AddExpense(
                dto.Name,
                dto.Amount,
                dto.Currency,
                dto.Type,
                dto.GroupId,
                dto.DeptorsIds,
                user
            );

            var res = await _mediator.Send(request);

            return Ok(res);
        }
    }
}
