using Core.Foo;
using EventStore.Client;
using Infrastructure.EventStore.Repository;
using MediatR;
using System.Text.Json;

namespace Application.Foo.Commands
{
    public record UpdateFoo(Guid Id, int SomeNumber) : IRequest<Guid>;

    public class UpdateFooHandler : IRequestHandler<UpdateFoo, Guid>
    {
        private readonly IEventStoreRepository<Core.Foo.Foo> _fooRepository;

        public UpdateFooHandler(IEventStoreRepository<Core.Foo.Foo> fooRepository)
        {
            _fooRepository = fooRepository;
        }

        public async Task<Guid> Handle(UpdateFoo request, CancellationToken cancellationToken)
        {
            var existing = await _fooRepository.Find(request.Id, cancellationToken);

            var evt = new FooUpdated(request.Id, request.SomeNumber);
            
            await _fooRepository.Append(request.Id, evt, cancellationToken);

            return request.Id;
        }
    }

}