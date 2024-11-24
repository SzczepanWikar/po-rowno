using API.Group.DTO;
using API.User.DTO;
using CommandModel.Group.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueryModel.Group.Handlers;

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
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetAll()
        {
            var user = HttpContext.Items["User"] as User;
            var request = new GetGroups(user!);

            var groups = await _mediator.Send(request);
            var res = groups.Select(e => GroupDto.FromEntity(e));

            return Ok(res);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetOne([FromRoute] Guid id)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new GetGroup(id, user!);

            var group = await _mediator.Send(request);
            var res = GroupDto.FromEntity(group);

            return Ok(res);
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

            return NoContent();
        }

        [HttpPatch("join")]
        public async Task<ActionResult> JoinToGroup([FromBody] JoinGroupDto dto)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new JoinGroup(dto.Code, user);

            await _mediator.Send(request);

            return NoContent();
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateGroupDto dto)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new UpdateGroupData(id, user, dto.Name, dto.Description, dto.OwnerId);

            await _mediator.Send(request);

            return NoContent();
        }

        [HttpPatch]
        [Route("{id}/ban-user")]
        public async Task<ActionResult> BanUser([FromRoute] Guid id, [FromBody] BanUserDto dto)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new BanUser(id, dto.Id, user);

            await _mediator.Send(request);

            return NoContent();
        }

        [HttpPatch]
        [Route("{id}/unban-user")]
        public async Task<ActionResult> UnbanUser([FromRoute] Guid id, [FromBody] UnbanUserDto dto)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new UnbanUser(id, dto.Id, user);

            await _mediator.Send(request);

            return NoContent();
        }

        [HttpPatch]
        [Route("{id}/leave")]
        public async Task<ActionResult> LeaveGroup([FromRoute] Guid id)
        {
            var user = HttpContext.Items["User"] as User;
            var request = new LeaveGroup(id, user);

            await _mediator.Send(request);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var user = HttpContext.Items["User"] as User;

            var request = new DeleteGroup(id, user);

            await _mediator.Send(request);

            return NoContent();
        }
    }
}
