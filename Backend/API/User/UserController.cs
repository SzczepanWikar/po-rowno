using API.User.DTO;
using API.User.ViewModels;
using Application.User.Commands;
using Core.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator) => _mediator = mediator;

        [HttpPost("sign-up")]
        public async Task<ActionResult<Guid>> SignUp([FromBody] SignUpDto dto)
        {
            var command = new SignUpUser(
                dto.Username,
                dto.Email,
                dto.Password,
                dto.ReapetedPassword
            );

            var res = await _mediator.Send(command);

            return new CreatedResult(null as string, res);
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<AppSignInResult>> SignIn([FromBody] SignInDto credentials)
        {
            var command = new SignInUser(credentials.Email, credentials.Password);

            var res = await _mediator.Send(command);

            return Ok(res);
        }

        [HttpPatch("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = HttpContext.Items["User"] as Core.User.User;

            var command = new ChangeUserPassword(dto.OldPassword, dto.NewPassword, user);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
