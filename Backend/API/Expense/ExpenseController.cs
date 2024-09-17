using API.Expense.DTO;
using Core.Common.PayPal.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WriteModel.Expense.Commands;

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

            return Created(String.Empty, res);
        }

        [HttpPost("payment")]
        public async Task<ActionResult<Guid>> CreateWithPayment(
            [FromBody] AddExpenseWithPaymentDto dto
        )
        {
            var user = HttpContext.Items["User"] as User;

            var request = new AddExpenseWithPayment(
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
        public async Task<ActionResult<OrderCapturedResponseDto>> Capture(
            [FromRoute] string orderId
        )
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
