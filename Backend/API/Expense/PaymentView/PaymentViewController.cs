using MediatR;
using Microsoft.AspNetCore.Mvc;
using WriteModel.Expense.Commands;

namespace API.Expense.PaymentView
{
    public class PaymentViewController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentViewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Success(Guid id)
        {
            await SendReq(id, true);
            return View();
        }

        public async Task<IActionResult> Cancel(Guid id)
        {
            await SendReq(id, false);
            return View();
        }

        private async Task SendReq(Guid id, bool success)
        {
            ApproveOrCancelExpense req = new(id, success);
            await _mediator.Send(req);
        }
    }
}
