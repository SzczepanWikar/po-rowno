using API.Group.DTO;
using Application.Group.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Group
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IMediator mediator, ILogger<GroupController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateGroupDto dto)
        {
            var user = HttpContext.Items["User"] as Core.User.User;
            var request = new CreateGroup(user!, dto.Name, dto.Description, dto.Currency);

            var res = await _mediator.Send(request);

            return Ok(res);
        }

        [HttpPatch]
        [Route("{id}/join-code")]
        public async Task<ActionResult> GenerateJoinGroupCode(
            [FromRoute] Guid id,
            [FromBody] GenerateJoinGroupCodeDto dto
        )
        {
            var user = HttpContext.Items["User"] as Core.User.User;
            var request = new GenerateJoinGroupCode(dto.ValidTo, id, user);

            await _mediator.Send(request);

            return Ok();
        }
    }
}
