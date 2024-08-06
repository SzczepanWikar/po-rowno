using System.Net;
using API.User.ViewModels;
using Application.User.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<Guid>> SignUp([FromBody] CreateUserViewModel viewModel)
        {
            var command = new SignUpUser(
                viewModel.Username,
                viewModel.Email,
                viewModel.Password,
                viewModel.ReapetedPassword
            );

            var res = await _mediator.Send(command);

            return new ObjectResult(new { StatusCode = (int)HttpStatusCode.Created, Value = res });
        }
    }
}
