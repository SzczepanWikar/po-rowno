using API.Expense.DTO;
using CommandModel.Expense.Commands;
using Core.Common.PayPal;
using Core.Expense;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QueryModel.Expense.Handler;

namespace API.Expense
{
    using User = Core.User.User;

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpenseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetAll(
            [FromQuery] GetExpensesQueryDto queryDto
        )
        {
            var user = HttpContext.Items["User"] as User;

            var query = new GetExpenses(
                queryDto.GroupId,
                user,
                queryDto.Page,
                queryDto.Take,
                queryDto.Ascending
            );

            var expenses = await _mediator.Send(query);
            var res =
                expenses.Select(ExpenseDto.FromEntity).AsEnumerable() ?? new List<ExpenseDto>();

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseDto>> GetOne(Guid id)
        {
            var user = HttpContext.Items["User"] as User;

            var query = new GetOneExpense(id, user);

            var expense = await _mediator.Send(query);
            var res = ExpenseDto.FromEntity(expense);

            return Ok(res);
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
                dto.Deptors,
                user
            );

            var res = await _mediator.Send(request);

            return Created(String.Empty, res);
        }

        [HttpPost("payment")]
        public async Task<ActionResult<ExpenseWithPaymentCreatedResult>> CreateWithPayment(
            [FromBody] AddExpenseWithPaymentDto dto
        )
        {
            var user = HttpContext.Items["User"] as User;

            var request = new AddExpenseWithPayment(
                dto.Name,
                dto.Amount,
                dto.Currency,
                dto.GroupId,
                dto.ReceiverId,
                user
            );

            var res = await _mediator.Send(request);

            return Created(String.Empty, res);
        }

        [HttpPatch("payment/{orderId}/capture")]
        public async Task<ActionResult<OrderCapturedResponse>> Capture([FromRoute] string orderId)
        {
            var user = HttpContext.Items["User"] as User;

            var request = new CapturePayment(orderId, user);

            var res = await _mediator.Send(request);

            return Created(String.Empty, res);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new RemoveExpense(id, user);

            await _mediator.Send(request);

            return NoContent();
        }
    }
}
