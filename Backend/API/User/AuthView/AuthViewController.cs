using Application.User.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.User.AuthView
{
    public class AuthViewController : Controller
    {
        private readonly IMediator _mediator;

        public AuthViewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> AccountActivation(Guid token)
        {
            try
            {
                var command = new ActivateAccount(token);
                await _mediator.Send(command);

                ViewBag.ActivationSucces = true;
            }
            catch (Exception ex)
            {
                ViewBag.ActivationSucces = false;
            }

            return View();
        }
    }
}
