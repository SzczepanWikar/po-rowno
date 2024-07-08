using Application.Foo.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FooController: ControllerBase
    {
        private readonly IMediator _mediator;
        public FooController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateFoo createFoo)
        {
            var command = new CreateFoo(createFoo.Name, createFoo.SomeNumber);
            var res = await _mediator.Send(command);
            return Ok(res);
        }

        [HttpPatch]
        [Route(":id")]
        public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateFooViewModel viewModel)
        {
            var command = new UpdateFoo(id, viewModel.SomeNumber);
            var res = await _mediator.Send(command);
            return Ok(res);
        }


        
    }

    public record UpdateFooViewModel(int SomeNumber);
}
