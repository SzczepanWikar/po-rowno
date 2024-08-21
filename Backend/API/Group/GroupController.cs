using API.Group.DTO;
using Application.Group.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Group
{
    using User = Core.User.User;

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
            var user = HttpContext.Items["User"] as User;
            var request = new CreateGroup(user!, dto.Name, dto.Description, dto.Currency);

            var res = await _mediator.Send(request);

            return new CreatedResult(null as string, res);
            ;
        }

        [HttpPatch]
        [Route("{id}/join-code")]
        public async Task<ActionResult> GenerateJoinGroupCode(
            [FromRoute] Guid id,
            [FromBody] GenerateJoinGroupCodeDto dto
        )
        {
            var user = HttpContext.Items["User"] as User;
            var request = new GenerateJoinGroupCode(dto.ValidTo, id, user);

            await _mediator.Send(request);

            return Ok();
        }

        [HttpPatch("join")]
        public async Task<ActionResult> JoinToGroup([FromBody] JoinGroupDto dto)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new JoinGroup(dto.Code, user);

            await _mediator.Send(request);

            return Ok();
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateGroupDto dto)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new UpdateGroupData(id, user, dto.Name, dto.Description, dto.OwnerId);

            await _mediator.Send(request);

            return Ok();
        }
    }
}
