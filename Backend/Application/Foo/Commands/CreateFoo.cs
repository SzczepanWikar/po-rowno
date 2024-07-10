using Core.Foo;
using EventStore.Client;
using Infrastructure.EventStore.Repository;
using MediatR;
using System.Text.Json;

namespace Application.Foo.Commands
{
    public record CreateFoo(string Name, int SomeNumber) : IRequest<Guid>;

    public class CreateFooHandler : IRequestHandler<CreateFoo, Guid>
    {
        private readonly IEventStoreRepository<Core.Foo.Foo> _fooRepository;

        public CreateFooHandler(IEventStoreRepository<Core.Foo.Foo> fooRepository)
        {
            _fooRepository = fooRepository;
        }
        public async Task<Guid> Handle(CreateFoo request, CancellationToken cancellationToken)
        {
            var evt = new FooCreated(Guid.NewGuid(), request.Name, request.SomeNumber);
            await _fooRepository.Create(evt.Id, evt, cancellationToken);

            return evt.Id;
        }
    }

}
