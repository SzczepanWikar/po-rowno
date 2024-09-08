using Core.Common.Exceptions;
using Core.Common.Projections;
using Core.User;
using Core.User.Events;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application.User.Commands
{
    using User = Core.User.User;

    public sealed record ResetPassword(string Email, string Password, string Code) : IRequest;

    public sealed class ResetPasswordHandler : IRequestHandler<ResetPassword>
    {
        private readonly IUserService _service;
        private readonly IIndexProjectionRepository _indexedEmailRepository;
        private readonly IIndexProjectionRepository _indexedCodeRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ResetPasswordHandler(
            IUserService service,
            IPasswordHasher<User> passwordHasher,
            [FromKeyedServices(InternalProjectionName.EmailIndex)]
                IIndexProjectionRepository indexedEmailRepository,
            [FromKeyedServices(InternalProjectionName.UserCodeIndex)]
                IIndexProjectionRepository indexedCodeRepository
        )
        {
            _service = service;
            _passwordHasher = passwordHasher;
            _indexedEmailRepository = indexedEmailRepository;
            _indexedCodeRepository = indexedCodeRepository;
        }

        public async Task Handle(ResetPassword request, CancellationToken cancellationToken)
        {
            var userIdFromEmail = await _indexedEmailRepository.GetOwnerId(request.Email);
            var userIdFromCode = await _indexedCodeRepository.GetOwnerId(request.Code);

            ValidateUserId(userIdFromEmail, userIdFromCode);

            var user = await _service.FindOneAsync(userIdFromCode.Value, cancellationToken);

            ValidateCode(request, user);

            var hashedPassword = _passwordHasher.HashPassword(user, request.Password);

            var @event = new UserPaswordChanged(user.Id, hashedPassword);
            await _service.AppendAsync(user.Id, @event, cancellationToken);

            var codeUsedEvent = new UserCodeUsed(user.Id, request.Code);
        }

        private static void ValidateUserId(Guid? userIdFromEmail, Guid? userIdFromCode)
        {
            if (!(userIdFromEmail.HasValue && userIdFromCode.HasValue))
            {
                throw new NotFoundException("User not found!");
            }

            if (userIdFromEmail.Value != userIdFromCode.Value)
            {
                throw new BadRequestException("Invalid code.");
            }
        }

        private static void ValidateCode(ResetPassword request, User user)
        {
            var validCode = user.Codes.Peek();

            if (!validCode.Check(request.Code, UserCodeType.ResetPassword))
            {
                throw new BadRequestException("Invalid code.");
            }
        }
    }
}
