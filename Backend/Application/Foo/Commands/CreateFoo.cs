using Core.Foo;
using Infrastructure.Email.Service;
using Infrastructure.EventStore.Repository;
using MediatR;

namespace Application.Foo.Commands
{
    public record CreateFoo(string Name, int SomeNumber) : IRequest<Guid>;

    public class CreateFooHandler : IRequestHandler<CreateFoo, Guid>
    {
        private readonly IEventStoreRepository<Core.Foo.Foo> _fooRepository;
        private readonly IEmailService _emailService;

        public CreateFooHandler(
            IEventStoreRepository<Core.Foo.Foo> fooRepository,
            IEmailService emailService
        )
        {
            _fooRepository = fooRepository;
            _emailService = emailService;
        }

        public async Task<Guid> Handle(CreateFoo request, CancellationToken cancellationToken)
        {
            var evt = new FooCreated(Guid.NewGuid(), request.Name, request.SomeNumber);
            await _fooRepository.Create(evt.Id, evt, cancellationToken);

            //var to = new List<ReceiverData>()
            //{
            //    new ReceiverData("User", "foo@example.pl")
            //};

            //var emailMessage = new EmailMessage(
            //    to,
            //    "test",
            //    "Lorem Ipsum",
            //    MimeKit.Text.TextFormat.Text
            //);

            //await _emailService.SendEmailAsync(emailMessage);

            return evt.Id;
        }
    }
}
