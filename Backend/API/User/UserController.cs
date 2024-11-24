using API.User.DTO;
using API.User.ViewModels;
using CommandModel.User.Commands;
using Core.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadModel.User.Handlers;

namespace API.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("yourself")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetYourself()
        {
            var user = HttpContext.Items["User"] as Core.User.User;

            GetUser request = new(user);
            var res = await _mediator.Send(request);
            var dto = UserDto.FromEntity(res);

            return Ok(dto);
        }

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

        [HttpPost("request-reset-password")]
        public async Task<ActionResult> RequestPasswordChange(
            [FromBody] RequestPasswordResetDto dto
        )
        {
            try
            {
                var command = new RequestPasswordReset(dto.Email);

                await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return NoContent();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AppSignInResult>> Refresh([FromBody] RefreshDto dto)
        {
            var command = new RefreshAccessToken(dto.refreshToken);

            var res = await _mediator.Send(command);

            return Ok(res);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var user = HttpContext.Items["User"] as Core.User.User;

            var command = new Logout(user);
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPatch("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = HttpContext.Items["User"] as Core.User.User;

            var command = new ChangeUserPassword(dto.OldPassword, dto.NewPassword, user);
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPatch("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var command = new ResetPassword(dto.Email, dto.Password, dto.Code);
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromBody] DeleteAccountDto dto)
        {
            var user = HttpContext.Items["User"] as Core.User.User;

            var command = new DeleteAccount(dto.Password, user);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
